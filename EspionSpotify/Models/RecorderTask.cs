using System.Threading;
using System.Threading.Tasks;

namespace EspionSpotify.Models
{
    internal class RecorderTask
    {
        public Task Task { get; set; }
        public IRecorder Recorder { get; set; }
        public CancellationTokenSource Token { get; set; }
    }
}