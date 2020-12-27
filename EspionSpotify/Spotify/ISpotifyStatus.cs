using EspionSpotify.Models;

namespace EspionSpotify.Spotify
{
    public interface ISpotifyStatus
    {
        Track CurrentTrack { get; set; }

        Track GetTrack();
    }
}
