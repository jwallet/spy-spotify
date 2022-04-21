using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EspionSpotify.Models;
using EspionSpotify.AudioSessions;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace EspionSpotify.AudioSessions
{
    public sealed class AudioThrottler: IDisposable
    {
        private bool _disposed;
        private const int DETECTED_SILENCE_MS = 500;
        private readonly object _lockObject;
        
        public const int BUFFER_TOTAL_SIZE_MS = 10_000;
        public const int BUFFER_THROTTLE_MS = 5_000;
        
        private readonly IMainAudioSession _audioSession;

        private CancellationTokenSource _cancellationTokenSource;
        private WasapiLoopbackCapture _waveIn;
        
        private readonly ConcurrentQueue<WaveBuffer> _queue;
        
        public bool Running { get; set; }
        
        public WaveFormat WaveFormat
        {
            get => _waveIn.WaveFormat; 
        }
        
        public AudioThrottler()
        {
        }

        public AudioThrottler(IMainAudioSession audioSession)
        {
            _queue = new ConcurrentQueue<WaveBuffer>();
            _audioSession = audioSession;
            _lockObject = new object();
        }

        public async Task<WaveBuffer> Dequeue(SilenceAnalyzer silence = SilenceAnalyzer.None)
        {
            switch (silence)
            {
                case SilenceAnalyzer.TrimStart:
                {
                    // prevents empty buffer
                    if (Peek(BUFFER_THROTTLE_MS) is null) return null;
                    lock (_lockObject)
                    {
                        for (var countSilence = _queue.Count(x => x.WithSilence); countSilence > 0;)
                        {
                            var result = _queue.TryDequeue(out var entry);
                            if (result && entry.WithSilence)
                            {
                                countSilence--;
                            }
                        }
                    }

                    return null;
                }
                default:
                {
                    lock (_lockObject)
                    {
                        var result = _queue.TryDequeue(out var entry);
                        return result ? entry : null;
                    }
                }
            }
        }

        public async Task DataAvailable(int waitForMs)
        {
            while (Peek(waitForMs) == null)
            {
                await Task.Delay(50);
            }
        }
        
        private WaveBuffer Peek(int waitForMs)
        {
            var nowMs = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            lock (_lockObject)
            {
                if (_queue.TryPeek(out var oldestEntry))
                {
                    var oldestEntryFromNow = TimeSpan.FromMilliseconds(nowMs)
                        .Subtract(TimeSpan.FromMilliseconds(oldestEntry.TimeStampMs));
                    if (oldestEntryFromNow.TotalMilliseconds > waitForMs)
                    {
                        return oldestEntry;
                    }
                }
            }

            return null;
        }

        private bool DiscardNextInQueue()
        {
            return _queue.TryDequeue(out var _);
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
        
        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (!Running) return;

            lock (_lockObject)
            {
                var received = new byte[e.BytesRecorded];
                Array.Copy(e.Buffer, 0, received, 0, e.BytesRecorded);
                var entry = new WaveBuffer
                {
                    Buffer = e.Buffer,
                    BytesRecorded = received,
                    BytesRecordedCount = e.BytesRecorded,
                    TimeStampMs = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                    WithSilence = received.TakeWhile(x => x == 0).Count() > (DETECTED_SILENCE_MS /1_000) * _waveIn.WaveFormat.AverageBytesPerSecond
                };
                _queue.Enqueue(entry);

                if (Peek(BUFFER_TOTAL_SIZE_MS) != null)
                {
                    var result = DiscardNextInQueue();
                    if (result) Console.Write(@"Nothing to remove in the audio throttler buffer.");
                }
            }
        }
        
        #region DisposeRecorder
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            // if (disposing) ForceStopRecording();

            _disposed = true;
        }

        #endregion DisposeRecorder
    }

    public class WaveBuffer
    {
        public double TimeStampMs;
        public byte[] Buffer;
        public byte[] BytesRecorded;
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