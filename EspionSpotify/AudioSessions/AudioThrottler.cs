using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EspionSpotify.Models;
using EspionSpotify.AudioSessions;
using NAudio.CoreAudioApi;
using NAudio.Utils;
using NAudio.Wave;

namespace EspionSpotify.AudioSessions
{
    public sealed class AudioThrottler: IAudioThrottler, IDisposable
    {
        private bool _disposed;
        private const int DETECTED_SILENCE_MS = 500;
        private readonly object _lockObject;
        private bool _dequeuing = false;
        
        private const int BUFFER_TOTAL_SIZE_IN_SECOND = 10;
        private const int BUFFER_THROTTLE_IN_SECOND = 5;
        
        private readonly IMainAudioSession _audioSession;

        private CancellationTokenSource _cancellationTokenSource;
        private WasapiLoopbackCapture _waveIn;
        private AudioCircularBuffer _buffer;
        
        private int BufferReadOffset => _waveIn.WaveFormat.AverageBytesPerSecond * BUFFER_THROTTLE_IN_SECOND;
        private int BufferMaxLength => _waveIn.WaveFormat.AverageBytesPerSecond * BUFFER_TOTAL_SIZE_IN_SECOND;
        private int SilenceAverageByteLength =>
            (DETECTED_SILENCE_MS / 1_000) * _waveIn.WaveFormat.AverageBytesPerSecond;
        
        public bool Running { get; set; }
        public WaveFormat WaveFormat => _waveIn.WaveFormat;

        public AudioThrottler(IMainAudioSession audioSession)
        {
            _audioSession = audioSession;
            _lockObject = new object();
        }
        
        public async Task Run(CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = cancellationTokenSource;

            if (_audioSession.AudioMMDevicesManager.AudioEndPointDevice == null) return;

            Running = true;

            _waveIn = new WasapiLoopbackCapture(_audioSession.AudioMMDevicesManager.AudioEndPointDevice);
            _waveIn.ShareMode = AudioClientShareMode.Shared;
            _waveIn.DataAvailable += WaveIn_DataAvailable;
            
            await Task.Delay(50);
            _waveIn.StartRecording();

            while (Running)
            {
                if (_cancellationTokenSource.IsCancellationRequested) return;
                await Task.Delay(100);
            }

            _waveIn.StopRecording();
        }

        public AudioWaveBuffer Read(SilenceAnalyzer silence = SilenceAnalyzer.None)
        {
            if (_dequeuing) return null;
            
            _dequeuing = true;
            AudioWaveBuffer result = null;

            switch (silence)
            {
                case SilenceAnalyzer.TrimStart:
                {
                    lock (_lockObject)
                    {
                        var readPeek = _buffer.Peek(out var dataPeek, 0, BufferReadOffset);
                        var read = BufferPositionWithoutSilence(dataPeek, readPeek);
                        if (read != 0)
                        {
                            _buffer.Advance(read);
                        }
                    }

                    break;
                }
                case SilenceAnalyzer.TrimEnd:
                {
                    lock (_lockObject)
                    {
                        var read = _buffer.Read(out var data, 0,
                            _buffer.Count);
                        if (read > 0)
                        {
                            result = ToAudioWaveBuffer(data, read);
                        }
                    }

                    break;
                }
                case SilenceAnalyzer.None:
                {
                    lock (_lockObject)
                    {
                        if (_buffer.Count >= BufferReadOffset)
                        {
                            var read = _buffer.Read(out var data, 0,
                                _waveIn.WaveFormat.AverageBytesPerSecond);
                            if (read > 0)
                            {
                                result = ToAudioWaveBuffer(data, read);
                            }
                        }
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(silence), silence, @"Cannot cast to Silence Analyzer. Not supported.");
            }

            _dequeuing = false;
            return result;
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (!Running) return;

            if (_buffer == null)
            {
                _buffer = new AudioCircularBuffer(BufferMaxLength);

            }
                
            lock (_lockObject)
            {
                _buffer.Write(e.Buffer, 0, e.BytesRecorded);
            }
        }

        private AudioWaveBuffer ToAudioWaveBuffer(byte[] data, int read)
        {
            return new AudioWaveBuffer
            {
                Buffer = data,
                BytesRecordedCount = read,
                WithSilence = WithSilence(data, read)
            };
        }
        
        private bool WithSilence(byte[] buffer, int count)
        {
            var received = new byte[count];
            Array.Copy(buffer, 0, received, 0, count);
            return received.TakeWhile(x => x == 0).Count() > SilenceAverageByteLength;
        }

        private int BufferPositionWithoutSilence(byte[] buffer, int count)
        {
            var received = new byte[count];
            Array.Copy(buffer, 0, received, 0, count);
            
            var consecutiveZero = 0;
            var firstAtPosition = 0;
            
            for (var i = 0; i < received.Length && consecutiveZero < SilenceAverageByteLength; i++)
            {
                if (received[i] == 0)
                {
                    if (consecutiveZero == 0)
                    {
                        firstAtPosition = i;
                    }
                    consecutiveZero++;
                }
                else consecutiveZero = 0;
            }

            return firstAtPosition != 0 ? firstAtPosition : 0;
        }


        #region Dispose
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _cancellationTokenSource.Cancel();
                _waveIn.Dispose();
            }

            _disposed = true;
        }

        #endregion DisposeRecorder
    }

    public class AudioWaveBuffer
    {
        public byte[] Buffer;
        public int BytesRecordedCount;
        public bool WithSilence;
    }

    public enum SilenceAnalyzer
    {  
        None,
        TrimEnd,
        TrimStart,
    }
}