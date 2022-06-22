using System;
using System.Threading;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace EspionSpotify.AudioSessions
{
    public class AudioLoopback: IAudioLoopback, IDisposable
    {
        private readonly bool _canDo = false;
        private bool _isDisposed = false;
        
        private const int BUFFER_TOTAL_SIZE_MS = 10_000;
        
        private readonly BufferedWaveProvider _bufferedWaveProvider;
        private readonly WasapiLoopbackCapture _waveIn;
        private readonly WaveOut _waveOut;
        
        private CancellationTokenSource _cancellationTokenSource;
        
        public bool Running { get; set; }

        public AudioLoopback(MMDevice currentEndpointDevice, MMDevice defaultEndpointDevice)
        {
            _canDo = currentEndpointDevice.ID != defaultEndpointDevice.ID;
            
            _waveIn = new WasapiLoopbackCapture(currentEndpointDevice);
            _waveIn.DataAvailable += OnDataAvailable;

            _bufferedWaveProvider = new BufferedWaveProvider(_waveIn.WaveFormat)
            {
                DiscardOnBufferOverflow = true,
                BufferDuration = TimeSpan.FromMilliseconds(BUFFER_TOTAL_SIZE_MS)
            };

            _waveOut = new WaveOut();
        }

        public async Task Run(CancellationTokenSource cancellationTokenSource)
        {
            if (!_canDo) return;
            
            _cancellationTokenSource = cancellationTokenSource;
            Running = true;

            _waveIn.StartRecording();
            _waveOut.Init(_bufferedWaveProvider);
            
            // TODO: Stop duplicate playback with user settings
            // _waveOut.Play();
            
            while (Running)
            {
                if (_cancellationTokenSource.IsCancellationRequested) return;
                await Task.Delay(100);
            }
            
            _waveOut.Stop();
            _waveIn.StopRecording();
        }
        
        private void OnDataAvailable(object sender, WaveInEventArgs waveInEventArgs)
        { 
            _bufferedWaveProvider.AddSamples(waveInEventArgs.Buffer,0, waveInEventArgs.BytesRecorded);
        }
        
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                _waveIn.Dispose();
                _waveOut.Dispose();
            }
        }
    }
}