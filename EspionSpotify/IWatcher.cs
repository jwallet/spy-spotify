using EspionSpotify.Spotify;
using System.Threading.Tasks;

namespace EspionSpotify
{
    public interface IWatcher
    {
        int CountSeconds { get; set; }
        ISpotifyHandler Spotify { get; set; }

        bool RecorderUpAndRunning { get; }
        bool IsTypeAllowed { get; }
        bool IsOldSong { get; }

        Task Run();
    }
}
