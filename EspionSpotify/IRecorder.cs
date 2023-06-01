using System.Threading;
using System.Threading.Tasks;
using EspionSpotify.Models;

namespace EspionSpotify
{
    public interface IRecorder
    {
        Track Track { get; }
        bool Running { get; }

        void Start();

        void Stop();

        void UpdateTrackPosition(int? position);

        Task Run(CancellationTokenSource token);

        void Dispose();
    }
}