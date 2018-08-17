using EspionSpotify.Models;
using System.Threading.Tasks;

namespace EspionSpotify.Spotify
{
    public interface ISpotifyStatus
    {
        Track Track { get; set; }

        Track GetTrack();
    }
}
