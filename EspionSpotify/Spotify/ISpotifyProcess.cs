using EspionSpotify.Models;
using System.Threading.Tasks;

namespace EspionSpotify.Spotify
{
    public interface ISpotifyProcess
    {
        Task<ISpotifyStatus> GetSpotifyStatus();
    }
}
