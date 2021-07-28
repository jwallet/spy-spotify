using EspionSpotify.Enums;
using EspionSpotify.Models;
using System.Threading.Tasks;

namespace EspionSpotify.API
{
    public interface IExternalAPI
    {
        ExternalAPIType GetTypeAPI { get; }
        bool IsAuthenticated { get; }
        Task Authenticate();
        void Reset();

        Task UpdateTrack(Track track);
    }
}
