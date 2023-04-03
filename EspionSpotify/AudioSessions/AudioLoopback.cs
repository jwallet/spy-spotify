using System;
using System.Threading;
using System.Threading.Tasks;
using EspionSpotify.AudioSessions.NAudio;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace EspionSpotify.AudioSessions
{
    public class AudioLoopback: IAudioLoopback, IDisposable
    {
        private readonly bool _canDo = false;
        private bool _disposed = false;
        
        private const int BUFFER_TOTAL_SIZE_MS = 5_000;
        
        private readonly BufferedWaveProvider _bufferedWaveProvider;
        private readonly IAudioLoopbackCapture _waveIn;
        private readonly IAudioWaveOut _audioLoopback;
        
        private CancellationTokenSource _cancellationTokenSource;
        
        public bool Running { get; set; }


        internal AudioLoopback(MMDevice currentEndpointDevice, MMDevice defaultEndpointDevice) : this(
            currentEndpointDevice,
            defaultEndpointDevice,
            new AudioLoopbackCapture(currentEndpointDevice),
            new AudioWaveOut())
        { }


        public AudioLoopback(MMDevice currentEndpointDevice, MMDevice defaultEndpointDevice, IAudioLoopbackCapture waveInCapture, IAudioWaveOut audioWaveOut)
        {
            _canDo = currentEndpointDevice.ID != defaultEndpointDevice.ID;
            
            _waveIn = waveInCapture;
            _waveIn.DataAvailable += OnDataAvailable;

            _bufferedWaveProvider = new BufferedWaveProvider(_waveIn.WaveFormat)
            {
                DiscardOnBufferOverflow = true,
                BufferDuration = TimeSpan.FromMilliseconds(BUFFER_TOTAL_SIZE_MS)
            };

            audioWaveOut.CreatePlayback(_bufferedWaveProvider);
            _audioLoopback = audioWaveOut;
        }

        public async Task Run(CancellationTokenSource cancellationTokenSource)
        {
            if (!_canDo) return;
            
            _cancellationTokenSource = cancellationTokenSource;
            Running = true;

            _waveIn.StartRecording();

            _audioLoopback.Play();
            
            while (Running)
            {
                if (_cancellationTokenSource.IsCancellationRequested) return;
                await Task.Delay(100);
            }
            
            _audioLoopback.Stop();
            _waveIn.StopRecording();
        }
        
        private void OnDataAvailable(object sender, WaveInEventArgs waveInEventArgs)
        { 
            _bufferedWaveProvider.AddSamples(waveInEventArgs.Buffer,0, waveInEventArgs.BytesRecorded);
        }

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
                if (_audioLoopback != null)
                {
                    _audioLoopback.Dispose();
                }
                if (_waveIn != null)
                {
                    _waveIn.StopRecording();
                    _waveIn.Dispose();
                }
            }

            _disposed = true;
        }
    }
}