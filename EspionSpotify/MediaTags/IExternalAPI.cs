using EspionSpotify.Models;
using System.Threading.Tasks;

namespace EspionSpotify.MediaTags
{
    public interface IExternalAPI
    {
        bool IsAuthenticated { get; }
        Task Authenticate();

        Task UpdateTrack(Track track);
    }
}
