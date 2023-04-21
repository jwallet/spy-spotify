using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EspionSpotify.AudioSessions.Audio
{
    public interface IAudioWaveOut
    {
        public void CreateSilencer(ISampleProvider waveProvider);
        public void CreatePlayback(IWaveProvider waveProvider);

        public void Play();
        public void Stop();

        public void Dispose();
    }
}
