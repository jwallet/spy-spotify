using EspionSpotify.Spotify;
using System.Threading.Tasks;

namespace EspionSpotify
{
    public interface IWatcher
    {
        int CountSeconds { get; set; }
        ISpotifyHandler Spotify { get; set; }

        Task Run();
    }
}
