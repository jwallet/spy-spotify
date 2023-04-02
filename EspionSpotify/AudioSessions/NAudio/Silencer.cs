using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EspionSpotify.AudioSessions.NAudio
{
    public class Silencer: ISilencer
    {
        private WaveOutEvent _waveOutEvent;
        public Silencer() { }
        public void CreateWaveOut(WaveFormat waveFormat)
        {
            var silenceProvider = new SilenceProvider(waveFormat).ToSampleProvider();
            _waveOutEvent = new WaveOutEvent();
            _waveOutEvent.Init(silenceProvider);
        }

        public void Play() => _waveOutEvent.Play();

        public void Stop() => _waveOutEvent.Stop();

        public void Dispose() => _waveOutEvent.Dispose();
    }
}
