using System.Threading;
using System.Threading.Tasks;

namespace EspionSpotify.AudioSessions
{
    public interface IAudioLoopback
    {
        bool Running { get; set; }

        Task Run(CancellationTokenSource cancellationTokenSource);
        void Dispose();
    }
}