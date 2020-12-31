using EspionSpotify.Models;
using System.Threading.Tasks;

namespace EspionSpotify.Spotify
{
    public interface ISpotifyStatus
    {
        Track CurrentTrack { get; set; }

        Task<Track> GetTrack();
    }
}
