using System.Threading.Tasks;
using EspionSpotify.Models;

namespace EspionSpotify.Spotify
{
    public interface ISpotifyStatus
    {
        Track CurrentTrack { get; set; }

        Task<Track> GetTrack();
    }
}