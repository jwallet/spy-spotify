using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;

namespace EspionSpotify.AudioSessions
{
    public interface IAudioThrottler
    {
        bool Running { get; set; }
        WaveFormat WaveFormat { get; }

        Task Run(CancellationTokenSource cancellationTokenSource);
        AudioWaveBuffer Read(SilenceAnalyzer silence = SilenceAnalyzer.None);
        void Dispose();
    }
}