using EspionSpotify.AudioSessions.Audio;
using EspionSpotify.Extensions;
using EspionSpotify.Models;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EspionSpotify.AudioSessions
{
    public sealed class AudioThrottler : IAudioThrottler, IDisposable
    {
        private bool _disposed;
        private readonly object _lockObject;

        // Defines the capture cycle in milliseconds
        public const int CAPTURE_CYCLE_MS = 100;
        // Defines a threshold for detecting silence
        public const sbyte SILENCE_AMPLITUDE_THRESHOLD = -20;
        // Defines how long to collect silence bytes samples before testing it agains the silence threshold
        public const int SILENCE_SAMPLER_MS = 100;
        // Defines the buffer length based on seconds (it gets combined with the sample rate * channel * 4 (short to bytes)
        public const int BUFFER_SIZE_IN_SECONDS = 4;
        // Defines the timeout to wait for an offset of WaveAverageBytesPerSecond before reading the buffer with GetData
        private const int READ_TIMEOUT_MS = 1000;

        private readonly IMainAudioSession _audioSession;

        private CancellationTokenSource _cancellationTokenSource;
        private readonly IAudioLoopbackCapture _waveIn;
        private readonly IAudioCircularBuffer _audioBuffer;
        private readonly IAudioWaveOut _silencer;

        private readonly IDictionary<Guid, long> _workerReadPositions;
        private readonly IDictionary<Guid, long> _workerStopPositions;

        private int WaveAverageBytesPerSecond => _waveIn.WaveFormat.AverageBytesPerSecond;
        private int BufferMaxLength => WaveAverageBytesPerSecond * _bufferSizeInSecond;

        public bool Running { get; set; }
        public WaveFormat WaveFormat => _waveIn.WaveFormat;

        private readonly int _readTimeoutMs = READ_TIMEOUT_MS;
        private readonly int _bufferSizeInSecond = BUFFER_SIZE_IN_SECONDS;
        private readonly sbyte _silenceAmplitudeThreshold = SILENCE_AMPLITUDE_THRESHOLD;
        private readonly int _silenceSamplerMs = SILENCE_SAMPLER_MS;
        private readonly int _captureCycleMs = CAPTURE_CYCLE_MS;

        internal AudioThrottler(IMainAudioSession audioSession) : this(
            audioSession,
            new AudioLoopbackCapture(audioSession.AudioMMDevicesManager.AudioEndPointDevice),
            new AudioWaveOut(),
            BUFFER_SIZE_IN_SECONDS,
            SILENCE_AMPLITUDE_THRESHOLD,
            SILENCE_SAMPLER_MS,
            READ_TIMEOUT_MS,
            CAPTURE_CYCLE_MS)
        { }

        public AudioThrottler(
            IMainAudioSession audioSession,
            IAudioLoopbackCapture waveIn,
            IAudioWaveOut audioWaveOut,
            int bufferTotalSizeInSeconds = BUFFER_SIZE_IN_SECONDS,
            sbyte silenceAmplitudeThrehold = SILENCE_AMPLITUDE_THRESHOLD,
            int silenceSamplerMs = SILENCE_SAMPLER_MS,
            int readTimeoutMs = 0,
            int captureCycleMs = 0)
        {
            _audioSession = audioSession;
            _lockObject = new object();
            _workerReadPositions = new Dictionary<Guid, long>();
            _workerStopPositions = new Dictionary<Guid, long>();

            _waveIn = waveIn;
            _waveIn.DataAvailable += WaveIn_DataAvailable;

            _audioBuffer = new AudioCircularBuffer(BufferMaxLength);

            var silenceProvider = new SilenceProvider(WaveFormat).ToSampleProvider();
            audioWaveOut.CreateSilencer(silenceProvider);
            _silencer = audioWaveOut;

            _readTimeoutMs = readTimeoutMs;
            _bufferSizeInSecond = bufferTotalSizeInSeconds;
            _silenceAmplitudeThreshold = silenceAmplitudeThrehold;
            _captureCycleMs = captureCycleMs;
            _silenceSamplerMs = silenceSamplerMs;
        }

        public async Task Run(CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = cancellationTokenSource;

            if (!_audioSession.IsAudioEndpointDeviceActive) return;

            Running = true;

            _waveIn.StartRecording();

            // silencer outputs silence in the stream
            _silencer.Play();

            while (Running)
            {
                if (_cancellationTokenSource.IsCancellationRequested) return;

                await Task.Delay(_captureCycleMs);
            }

            _silencer.Stop();

            _waveIn.StopRecording();
        }

        public async Task<bool> WaitForWorkerReadPositionReadiness(Guid identifier, int timeout)
        {
            return await WaitForWorkerPositionReadiness(_workerReadPositions, identifier, timeout);
        }

        public async Task<bool> WaitForWorkerStopPositionReadiness(Guid identifier, int timeout)
        {
            return await WaitForWorkerPositionReadiness(_workerStopPositions, identifier, timeout);
        }

        private async Task<bool> WaitForWorkerPositionReadiness(IDictionary<Guid, long> workers, Guid identifier,  int timeout)
        {
            const int waitTimeMs = CAPTURE_CYCLE_MS;
            var timeoutLeft = timeout == -1 ? 0 : timeout;
            var isReady = false;
            var wait = true;

            while (wait)
            {
                var offsetNeeded = WaveAverageBytesPerSecond * 3;
                var hasWorkerPosition = workers.TryGetValue(identifier, out var lastReadPosition);

                // if the worker's position is unavailable, it means it has not been initialized yet
                if (!hasWorkerPosition)
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

                await Task.Delay(_captureCycleMs);
            }

            return isReady;
        }

        private long? MoveWorkerPositionToDetectedSilence(long? workerPosition)
        {
            long? offset = null;
            if (workerPosition.HasValue)
            {
                var desiredOffset = WaveAverageBytesPerSecond;
                var fromPosition = Math.Max(0, workerPosition.Value - desiredOffset);
                var toPosition = (int)Math.Min(_audioBuffer.TotalBytesWritten, workerPosition.Value + desiredOffset);
                lock (_lockObject)
                {
                    _audioBuffer.Read(out var buffer, 0,_audioBuffer.MaxLength);
                    offset = DetectSilencePosition(buffer, fromPosition, toPosition);
                }
            }

            return offset;
        }

        public async Task<AudioWaveBuffer> GetData(Guid identifier) => await GetData(identifier, -1);

        public async Task<AudioWaveBuffer> GetDataStart(Guid identifier, bool detectSilence)
        {
            _workerReadPositions.TryGetValue(identifier, out var readPosition);

            if (detectSilence)
            {
                await WaitForWorkerReadPositionReadiness(identifier, _readTimeoutMs);

                var offset = MoveWorkerPositionToDetectedSilence(readPosition);
                // detected silence will move the worker read position to the offset position
                if (offset.HasValue)
                {
                    _workerReadPositions[identifier] = offset.Value;
                }
            }

            return await GetDataTo(identifier, -1);
        }

        public async Task<AudioWaveBuffer> GetDataEnd(Guid identifier, bool detectSilence)
        {
            _workerStopPositions.TryGetValue(identifier, out var endingPosition);

            if (detectSilence)
            {
                await WaitForWorkerStopPositionReadiness(identifier, _readTimeoutMs);

                var offset = MoveWorkerPositionToDetectedSilence(endingPosition);
                // detected silence will move the worker stop position to the offset position
                if (offset.HasValue)
                {
                    endingPosition = offset.Value;
                    _workerStopPositions[identifier] = offset.Value;
                }
            }

            return await GetDataTo(identifier, endingPosition);
        }

        private async Task<AudioWaveBuffer> GetDataTo(Guid identifier, long positionToReach) => await GetData(identifier, positionToReach);

        private async Task<AudioWaveBuffer> GetData(Guid identifier, long forcePosition)
        {
            byte[] buffer;
            int bytesRead;

            // 1: Bypass if forcePosition is set
            // 2: Wait for data to be available in the circular buffer
            if (forcePosition != -1 || await WaitForWorkerReadPositionReadiness(identifier, _readTimeoutMs))
            {
                lock (_lockObject)
                {
                    // get the current read position for the worker, or set it to the silence offset if it has not been initialized yet
                    _workerReadPositions.TryGetValue(identifier, out var readPosition);

                    // read data from the circular buffer starting at the worker's read position
                    var bytesAvailableAfterOffset = _audioBuffer.TotalBytesWritten - (readPosition + (WaveAverageBytesPerSecond * 2));
                    var bytesAvailableOrMaxDefault = bytesAvailableAfterOffset > 0
                        ? (int)Math.Min(WaveAverageBytesPerSecond, bytesAvailableAfterOffset)
                        : WaveAverageBytesPerSecond;
                    var readCount = forcePosition == -1 ? bytesAvailableOrMaxDefault : (int)Math.Max(0, forcePosition - readPosition);
                    bytesRead = _audioBuffer.Read(out buffer, readPosition, readCount);

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

        //private void TrimEndBufferForSilence(ref byte[] buffer)
        //{
        //    var silenceAt = DetectSilencePosition(buffer, 0, _audioBuffer.MaxLength);
        //    if (silenceAt.HasValue)
        //    {
        //        var sequenceToRemove = buffer.SubArray(silenceAt.Value, buffer.Length);
        //        buffer = buffer.TrimEnd(sequenceToRemove);
        //    }
        //}

        public void AddWorker(Guid identifier)
        {
            _workerReadPositions.Add(identifier, _audioBuffer.TotalBytesWritten);
        }

        public long? GetWorkerPosition(Guid identifier)
        {
            return _workerReadPositions.TryGetValue(identifier, out var position) ? position : null;
        }

        public void StopWorker(Guid identifier)
        {
            _workerStopPositions.Add(identifier, _audioBuffer.TotalBytesWritten);
        }

        public bool StopWorkerExist(Guid identifier)
        {
            return _workerStopPositions.TryGetValue(identifier, out var position);
        }

        public void RemoveWorker(Guid identifier)
        {
            if (_workerReadPositions.TryGetValue(identifier, out var position))
            {
                _workerReadPositions.Remove(identifier);
            }
            if (_workerStopPositions.TryGetValue(identifier, out var stopPosition))
            {
                _workerStopPositions.Remove(identifier);
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

        public int? DetectSilencePosition2(byte[] buffer, int fromPosition, int toPosition)
        {
            short[] values = new short[buffer.Length / 2];

            Buffer.BlockCopy(buffer, 0, values, 0, buffer.Length);

            float[] samples = values.Select(x => x / (short.MaxValue + 1f)).ToArray();

            int silenceCount = samples.Count(x => AudioUtil.IsSilence(x, -40));

            var t = DetectSilencePosition2(buffer, fromPosition, toPosition);
            return null;
        }

        public long? DetectSilencePosition(byte[] buffer, long fromPosition, long toPosition)
        {
            var valuesLength = buffer.Length / 2;
            var minSilenceDurationMs = _silenceSamplerMs;
            var from = fromPosition / 2;
            var to = toPosition / 2;

            var sampleCount = to - from;
            var samples = new float[sampleCount];
            var values = new short[valuesLength];

            var offset = from % samples.Length;

            try
            {
                Buffer.BlockCopy(buffer, 0, values, 0, buffer.Length);

                // Convert short data to float samples
                for (var i = 0; i < samples.Length; i++)
                {
                    var value = values[offset + i];
                    var amplitude = value / (short.MaxValue + 1f);
                    samples[i] = amplitude;
                }

                //var kk = values.Where(x => x < 2).ToArray();

                // Find the median position of the silence
                var a = samples.Select((value, index) => new { value, index });
                var b = a.Where(x => AudioUtil.IsSilence(x.value, _silenceAmplitudeThreshold));
                var c = b.GroupWhile((x, y) => y.index - x.index == 1);
                var d = c.Select(x => new { position = (x.Median(x => x.index) * 2) + fromPosition, count = x.Count() });
                var e = d.OrderBy(x => x.count);
                var f = e.FirstOrDefault(x => x.count > _silenceSamplerMs);
                Console.WriteLine(f?.position);
                return f?.position;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return null;
            }
        }

        public int? DetectSilencePosition7(byte[] buffer, int fromPosition, int toPosition)
        {
            WaveFormat waveFormat = _waveIn.WaveFormat;
            float threshold = _silenceAmplitudeThreshold;

            var minSilenceDurationMs = _silenceSamplerMs;
            var sampleCount = (toPosition - fromPosition) / waveFormat.BlockAlign;
            var offset = (fromPosition % buffer.Length);
            var samples = new float[sampleCount];

            // Convert byte data to float samples
            for (var i = 0; i < sampleCount; i++)
            {
                var sampleOffset = offset + i * waveFormat.BlockAlign;
                samples[i] = BitConverter.ToSingle(buffer, sampleOffset);
            }

            // Calculate average volume of samples
            var volume = 0f;
            for (var i = 0; i < sampleCount; i++)
            {
                volume += Math.Abs(samples[i]);
            }
            volume /= sampleCount;

            // Check if volume is below the threshold
            if (volume >= threshold)
            {
                return null;
            }

            // Find the median position of the silence
            var silencePositions = Enumerable
                .Range(0, sampleCount)
                .Where(i => Math.Abs(samples[i]) < threshold)
                .Select(i => offset + i * waveFormat.BlockAlign)
                .OrderBy(pos => pos)
                .ToList();

            if (silencePositions.Count == 0)
            {
                return null;
            }

            var silencePositionInBytes = silencePositions[silencePositions.Count / 2];
            return silencePositionInBytes;
        }

        public bool DetectSilencePosition4(byte[] buffer, int offset, int count, out int silenceDurationMs)
        {
            WaveFormat waveFormat = _waveIn.WaveFormat;
            float threshold = _silenceAmplitudeThreshold;
            var minSilenceDurationMs = _silenceSamplerMs;

            var sampleCount = count / waveFormat.BlockAlign;
            var samples = new float[sampleCount];

            // Convert byte data to float samples
            for (var i = 0; i < sampleCount; i++)
            {
                var sampleOffset = offset + i * waveFormat.BlockAlign;
                samples[i] = BitConverter.ToSingle(buffer, sampleOffset);
            }

            // Calculate average volume of samples
            var volume = 0f;
            for (var i = 0; i < sampleCount; i++)
            {
                volume += Math.Abs(samples[i]);
            }
            volume /= sampleCount;

            // Check if volume is below the threshold
            if (volume >= threshold)
            {
                silenceDurationMs = 0;
                return false;
            }

            // Find the duration of silence
            var duration = 0;
            for (var i = 0; i < sampleCount; i++)
            {
                if (Math.Abs(samples[i]) >= threshold)
                {
                    break;
                }
                duration += waveFormat.BlockAlign;
            }

            silenceDurationMs = duration / waveFormat.BlockAlign;

            return silenceDurationMs >= minSilenceDurationMs;
        }

        public int? DetectSilencePosition3(byte[] buffer, int fromPosition, int toPosition)
        {
            List<int> silencePositions = new List<int>();

            int blockDurationMs = _silenceSamplerMs;
            float threshold = _silenceAmplitudeThreshold;

            int bytesPerSample = _waveIn.WaveFormat.BitsPerSample / 8;
            int samplesPerBlock = (int)Math.Round(_waveIn.WaveFormat.AverageBytesPerSecond * blockDurationMs / 1000.0 / bytesPerSample);


            for (int i = fromPosition; i < toPosition; i += samplesPerBlock * bytesPerSample)
            {
                float rms = 0;

                for (int j = i; j < i + samplesPerBlock * bytesPerSample && j < toPosition; j += bytesPerSample)
                {
                    float sample = BitConverter.ToInt16(buffer, j) / 32768f;
                    rms += sample * sample;
                }

                rms = (float)Math.Sqrt(rms / samplesPerBlock);

                if (rms < threshold)
                {
                    silencePositions.Add(i + samplesPerBlock * bytesPerSample);
                }
            }

            return silencePositions.Count > 0 ? (int?)silencePositions.Median() : null;
        }

        private int? DetectSilencePosition18(byte[] buffer, int fromPosition, int toPosition)
        {
            if (!Running) return null;

            // Calculate the number of bytes per sample (based on the format)
            var bytesPerSample = ((_waveIn.WaveFormat.BitsPerSample) / 8); // usually  or 8, but could be 4 or 1

            // Calculate the number of bytes per sampler: sampler are X ms long
            var bytesPerSamplerMs = (_waveIn.WaveFormat.SampleRate * _waveIn.WaveFormat.Channels * bytesPerSample) * (1000 / _silenceSamplerMs);

            var silencePositions = new List<int>();

            // Loop through the audio data in the buffer, processing sampler X ms of audio at a time
            for (var i = fromPosition; Running && i < toPosition; i += bytesPerSamplerMs)
            {
                var amplitudeSum = 0f;
                var sampleCount = 0;

                // Create a WaveBuffer from the byte array for efficient processing
                var waveBuffer = new WaveBuffer(buffer);

                // Loop through sampler X ms of audio data and calculate the average amplitude
                for (var j = i; j < i + bytesPerSamplerMs && j < toPosition; j += bytesPerSample)
                {
                    float amplitude = 0;

                    // Calculate the amplitude of the current sample
                    // IEEE float formats use floating-point samples: is 4 bytes
                    for (var k = 0; k < bytesPerSample; k += _waveIn.WaveFormat.BlockAlign)
                    {
                        // Convert the bytes to a float sample since we use IEEE float
                        var sample = BitConverter.ToSingle(buffer, j + k);
                        amplitude += Math.Abs(sample);
                    }

                    if (amplitude != 0)
                    {
                        amplitude /= (float)bytesPerSample / (float)_waveIn.WaveFormat.BlockAlign;
                    }

                    // Add the amplitude to the sum
                    amplitudeSum += amplitude;
                    sampleCount++;
                }

                // Calculate the average amplitude for the sampler X ms of audio
                var averageAmplitude = amplitudeSum / sampleCount;

                // If the average amplitude is below the silence threshold, consider it to be silence
                if (averageAmplitude < _silenceAmplitudeThreshold)
                {
                    silencePositions.Add(i);
                }
            }

            return silencePositions.Count > 0 ? (int?)silencePositions.Median() : null;
        }

        // Define a function to calculate the RMS amplitude of a buffer
        public float CalculateRMSAmplitude(byte[] buffer)
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
                    _waveIn.StopRecording();
                    _waveIn.Dispose();
                }
                _workerReadPositions.Clear();
                _workerStopPositions.Clear();
            }

            _disposed = true;
        }

        #endregion DisposeRecorder
    }
}