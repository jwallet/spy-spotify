using EspionSpotify.Models;
using EspionSpotify.Spotify;
using System.Threading.Tasks;

namespace EspionSpotify
{
    public interface IWatcher
    {
        int CountSeconds { get; set; }
        ISpotifyHandler Spotify { get; set; }

        bool RecorderUpAndRunning { get;  }
        bool NumTrackActivated { get; }
        bool AdPlaying { get; }
        string SongTitle { get; }
        bool IsTypeAllowed { get; }
        bool IsOldSong { get; }

        void Run();
    }
}
