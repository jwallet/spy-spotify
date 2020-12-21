using EspionSpotify.Models;
using System.Threading.Tasks;

namespace EspionSpotify.MediaTags
{
    public interface IExternalAPI
    {
        bool IsAuthenticated { get; }

        Task UpdateTrack(Track track);
    }
}
