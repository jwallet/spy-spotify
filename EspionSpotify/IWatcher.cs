using System.Threading.Tasks;
using EspionSpotify.Spotify;

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