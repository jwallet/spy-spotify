using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EspionSpotify.AudioSessions.Audio
{
    public interface IAudioLoopbackCapture
    {
        public CaptureState CaptureState { get; }
        public WaveFormat WaveFormat { get; }
        public event EventHandler<WaveInEventArgs> DataAvailable;
        public event EventHandler<StoppedEventArgs> RecordingStopped;
        public void StartRecording();
        public void StopRecording();

        public void Dispose();
    }
}
