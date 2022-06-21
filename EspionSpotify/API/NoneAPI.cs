using System.Threading.Tasks;
using EspionSpotify.Enums;
using EspionSpotify.Models;

namespace EspionSpotify.API
{
    public class NoneAPI: IExternalAPI
    {
        public ExternalAPIType GetTypeAPI => ExternalAPIType.None;
        public bool IsAuthenticated => true;
        public Task Authenticate()
        {
            return Task.CompletedTask;
        }

        public void Reset()
        {
        }

        public Task<bool> UpdateTrack(Track track)
        {
            return Task.FromResult(true);
        }
    }
}