using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EspionSpotify.AudioSessions.NAudio
{
    public interface ISilencer
    {
        public void CreateWaveOut(WaveFormat waveFormat);

        public void Play();
        public void Stop();

        public void Dispose();
    }
}
