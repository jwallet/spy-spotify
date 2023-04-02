using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EspionSpotify.Models;
using EspionSpotify.AudioSessions;
using EspionSpotify.Enums;
using EspionSpotify.Extensions;
using NAudio.CoreAudioApi;
using NAudio.Utils;
using NAudio.Wave;
using Unosquare.Swan;
using EspionSpotify.AudioSessions.NAudio;

namespace EspionSpotify.AudioSessions
{
    public sealed class AudioThrottler: IAudioThrottler, IDisposable
    {
        private bool _disposed;
        private readonly object _lockObject;

        // Defines the capture cycle in milliseconds
        public const int CAPTURE_CYCLE_MS = 100;
        // Defines the delay to wait before starting the capture in milliseconds, this is give time for the silence to be applied.
        public const int DELAYED_CAPTURE_MS = 100;
        // Defines a threshold for detecting silence (you may need to adjust this value based on your specific use case)
        public const float SILENCE_THRESHOLD = 0.01f;
        // Defines the buffer length based on seconds (it gets combined with the sample rate * channel * 4 (short to bytes)
        public const int BUFFER_SIZE_IN_SECONDS = 4;
        // Defines the timeout to wait for an offset of WaveAverageBytesPerSecond before reading the buffer with GetData
        private const int READ_TIMEOUT_MS = 1000;
        
        private readonly IMainAudioSession _audioSession;

        private CancellationTokenSource _cancellationTokenSource;
        private readonly IAudioLoopbackCapture _waveIn;
        private readonly IAudioCircularBuffer _audioBuffer;
        private readonly IDictionary<Guid, int> _workerReadPositions;
        private readonly ISilencer _silencer;

        private int WaveAverageBytesPerSecond => _waveIn.WaveFormat.AverageBytesPerSecond;
        private int BufferMaxLength => WaveAverageBytesPerSecond * _bufferSizeInSecond;
        
        public bool Running { get; set; }
        public WaveFormat WaveFormat => _waveIn.WaveFormat;

        private readonly int _readTimeoutMs = READ_TIMEOUT_MS;
        private readonly int _bufferSizeInSecond = BUFFER_SIZE_IN_SECONDS;
        private readonly float _silenceThreshold = SILENCE_THRESHOLD;
        private readonly int _captureCycleMs = CAPTURE_CYCLE_MS;
        private readonly int _delayedCaptureMs = DELAYED_CAPTURE_MS;

        internal AudioThrottler(IMainAudioSession audioSession): this(
            audioSession,
            new AudioLoopbackCapture(audioSession.AudioMMDevicesManager.AudioEndPointDevice),
            new Silencer(),
            READ_TIMEOUT_MS,
            BUFFER_SIZE_IN_SECONDS,
            SILENCE_THRESHOLD,
            CAPTURE_CYCLE_MS,
            DELAYED_CAPTURE_MS)
        { }

        public AudioThrottler(
            IMainAudioSession audioSession,
            IAudioLoopbackCapture waveIn,
            ISilencer silenceProvider,
            int readTimeoutMs = 0,
            int bufferTotalSizeInSeconds = BUFFER_SIZE_IN_SECONDS,
            float silenceThrehold = SILENCE_THRESHOLD,
            int captureCycleMs = CAPTURE_CYCLE_MS,
            int delayedCaptureMs = 0)
        {
            _audioSession = audioSession;
            _lockObject = new object();
            _workerReadPositions = new Dictionary<Guid, int>();

            _waveIn = waveIn;
            _waveIn.DataAvailable += WaveIn_DataAvailable;

            _audioBuffer = new AudioCircularBuffer(BufferMaxLength);

            silenceProvider.CreateWaveOut(WaveFormat);
            _silencer = silenceProvider;

            _readTimeoutMs = readTimeoutMs;
            _bufferSizeInSecond = bufferTotalSizeInSeconds;
            _silenceThreshold = silenceThrehold;
            _captureCycleMs = captureCycleMs;
            _delayedCaptureMs = delayedCaptureMs;
        }
        
        public async Task Run(CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = cancellationTokenSource;

            if (!_audioSession.IsAudioEndpointDeviceActive) return;

            Running = true;

            // silencer outputs silence in the stream
            _silencer.Play();

            await Task.Delay(_delayedCaptureMs);

            _waveIn.StartRecording();


            while (Running)
            {
                if (_cancellationTokenSource.IsCancellationRequested) return;

                Thread.Sleep(_captureCycleMs);
            }

            _silencer.Stop();

            _waveIn.StopRecording();
        }
        
        public async Task<bool> WaitForWorkerPositionReady(Guid identifier, int timeout)
        {
            const int waitTimeMs = 100;
            var timeoutLeft = timeout == -1 ? 0 : timeout;
            var isReady = false;
            var wait = true;

            while (wait)
            {
                var offsetNeeded = WaveAverageBytesPerSecond;
                var lastReadPosition = GetWorkerPosition(identifier);

                // if the worker's read position is null, it means it has not been initialized yet
                if (lastReadPosition == null)
                {
                    wait = false;
                    isReady = false;
                }
                else
                {
                    var bufferPosition = _audioBuffer.TotalBytesWritten;
                    var currentOffset = bufferPosition - lastReadPosition;
                    timeoutLeft = Math.Max(0, timeoutLeft - waitTimeMs);

                    var waiting = Running && (timeout == -1 || timeoutLeft > 0);
                    isReady = bufferPosition > lastReadPosition && currentOffset > offsetNeeded;
                    wait = waiting && !isReady;
                }

                await Task.Delay(100);
            }

            return isReady;
        }

        private int? GetSilenceOffset()
        {
            int? offset;
            lock (_lockObject)
            {
                _audioBuffer.Read(out var buffer, 0, _audioBuffer.MaxLength);
                offset = DetectSilencePosition(buffer);
            }
            return offset;
        }

        public async Task<AudioWaveBuffer> GetData(Guid identifier)
        {
            byte[] buffer;
            int bytesRead;

            // Wait for data to be available in the circular buffer
            if (await WaitForWorkerPositionReady(identifier, _readTimeoutMs))
            {
                lock (_lockObject)
                {
                    // get the current read position for the worker, or set it to the silence offset if it has not been initialized yet
                    _workerReadPositions.TryGetValue(identifier, out var readPosition);

                    // read data from the circular buffer starting at the worker's read position
                    bytesRead = _audioBuffer.Read(out buffer, readPosition, WaveAverageBytesPerSecond);

                    // Update the worker's read position
                    readPosition += bytesRead;

                    _workerReadPositions[identifier] = readPosition;
                }
            }
            else
            {
                buffer = new byte[] { };
                bytesRead = 0;
            }

            return new AudioWaveBuffer()
            {
                Buffer = buffer,
                BytesRecordedCount = bytesRead,
            };
        }

        public void TrimEndBufferForSilence(ref byte[] buffer)
        {
            var silenceAt = DetectSilencePosition(buffer);
            if (silenceAt.HasValue)
            {
                var sequenceToRemove = buffer.SubArray(silenceAt.Value, buffer.Length);
                buffer = buffer.TrimEnd(sequenceToRemove);
            }
        }

        public void AddWorker(Guid identifier)
        {
            //var markPosition = _audioBuffer.WritePosition;

            // wait buffer readyness after marking the position
            //_bufferReadyEvent.Wait(1000);

            // valid silence if present in the buffer
            //var silenceOffset = GetSilenceOffset();
            // if silenceOffset has a value it might be greater than the current mark, so it will result in a negative value, otherwise use mark position
            //var offset = silenceOffset.HasValue ? markPosition - silenceOffset.Value : markPosition;
            // count all bytes before the current cycle to be able to use a read position based on the totalBytesWritten
            //var bytesWrittenOnPreviousCycles = (int)(_audioBuffer.TotalBytesWritten - (_audioBuffer.TotalBytesWritten % _audioBuffer.MaxLength));
            //var marker = bytesWrittenOnPreviousCycles + markPosition;
            // create workers to start with an offset
            _workerReadPositions.Add(identifier, _audioBuffer.TotalBytesWritten);
        }

        public int? GetWorkerPosition(Guid identifier)
        {
            return _workerReadPositions.TryGetValue(identifier, out var position) ? position : null;
        }

        public void RemoveWorker(Guid identifier)
        {
            if (_workerReadPositions.TryGetValue(identifier, out var position))
            {
                _workerReadPositions.Remove(identifier);
            }
        }
        
        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (!Running) return;

            lock (_lockObject)
            {
                _audioBuffer.Write(e.Buffer, 0, e.BytesRecorded);
            }
        }

        //private int? BufferPositionOfSilence(byte[] buffer)
        //{
        //    var received = new byte[buffer.Length];
        //    Array.Copy(buffer, 0, received, 0, buffer.Length);
        //    var atPositionList = new List<int>();
            
        //    var bouncePerSample = (int) (SilenceAverageShortLength / 2.0);
        //    for (var i = 0; i < received.Length; i += bouncePerSample)
        //    {
        //        var sample = new short[SilenceAverageShortLength];
        //        var readCount = Math.Min(SilenceAverageShortLength, received.Length - i);
        //        Array.Copy(received, i, sample, 0, readCount);

        //        var average = sample.Average(x => x);
        //        if (average > 0) continue;

        //        atPositionList.Add(i);
        //    }

        //    return atPositionList.Any() ? atPositionList.First() : null;
        //}

        private int? DetectSilencePosition(byte[] buffer)
        {
            if (!Running) return null;

            // Calculate the number of bytes per sample (based on the format)
            var bytesPerSample = _waveIn.WaveFormat.BitsPerSample / 8 * _waveIn.WaveFormat.Channels;

            // Loop through the audio data in the buffer
            for (var i = 0; Running && i < buffer.Length; i += bytesPerSample)
            {
                // Calculate the amplitude of the current sample
                float amplitude = 0;
                for (var j = 0; j < bytesPerSample; j += 2)
                {
                    var sample = (short)(buffer[i + j] | buffer[i + j + 1] << 8);
                    amplitude += Math.Abs(sample / 32768f);
                }
                
                amplitude /= (float)bytesPerSample / 2f;

                // If the average amplitude is below the silence threshold, consider it to be silence
                if (amplitude < _silenceThreshold)
                {
                    return i;
                }
            }

            return null;
        }
        
        // Define a function to calculate the RMS amplitude of a buffer
        private float CalculateRMSAmplitude(byte[] buffer)
        {
            // Calculate the number of samples in the buffer (assuming 16-bit samples)
            int numSamples = buffer.Length / 2;

            // Define variables to hold the sum of squared samples and the RMS amplitude
            float sumSquared = 0;
            float rmsAmplitude = 0;

            // Iterate over the samples in the buffer and calculate the sum of squared samples
            for (int i = 0; Running && i < numSamples; i++)
            {
                // Get the 16-bit sample value from the byte buffer (little-endian encoding)
                short sampleValue = BitConverter.ToInt16(buffer, i * 2);

                // Convert the sample value to a float in the range [-1, 1]
                float sample = (float)sampleValue / 32768.0f;

                // Square the sample value and add it to the sum of squared samples
                sumSquared += sample * sample;
            }

            // Calculate the RMS amplitude as the square root of the average of the squared samples
            if (numSamples > 0)
            {
                rmsAmplitude = (float)Math.Sqrt(sumSquared / numSamples);
            }

            return rmsAmplitude;
        }


        #region Dispose
        public void Dispose()
        {
            Running = false;

            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                }
                if (_silencer != null)
                {
                    _silencer.Dispose();
                }
                if (_waveIn != null)
                {
                   _waveIn.Dispose();
                }
                _workerReadPositions.Clear();
            }

            _disposed = true;
        }

        #endregion DisposeRecorder
    }
}