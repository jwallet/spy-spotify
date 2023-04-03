using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EspionSpotify.AudioSessions.NAudio
{
    public class AudioWaveOut: IAudioWaveOut, IDisposable
    {
        private bool _disposed;
        private WaveOutEvent _waveOut;

        public AudioWaveOut() { }

        public void CreateSilencer(ISampleProvider waveProvider)
        {
            _waveOut = new WaveOutEvent();
            _waveOut.Init(waveProvider);
        }

        public void CreatePlayback(IWaveProvider waveProvider)
        {
            _waveOut = new WaveOutEvent();
            _waveOut.Init(waveProvider);
        }

        public void Play() => _waveOut.Play();

        public void Stop() => _waveOut.Stop();

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
                _waveOut.Dispose();
            }
            _disposed = true;
        }
    }
}
