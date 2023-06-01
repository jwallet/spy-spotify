using System.Threading.Tasks;
using EspionSpotify.Spotify;

namespace EspionSpotify
{
    public interface IWatcher
    {
        ISpotifyHandler Spotify { get; set; }

        bool RecorderUpAndRunning { get; }
        bool IsTypeAllowed { get; }

        Task Run();
    }
}