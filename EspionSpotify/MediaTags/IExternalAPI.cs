using EspionSpotify.Models;
using System.Threading.Tasks;

namespace EspionSpotify.MediaTags
{
    public interface IExternalAPI
    {
        Task<bool> UpdateTrack(Track track);
    }
}
