using EspionSpotify.Models;
using System.Threading.Tasks;

namespace EspionSpotify.MediaTags
{
    public interface ILastFMAPI
    {
        Task<bool> UpdateTrack(Track track);

        void MapLastFMTrackToTrack(Track track, LastFMTrack trackExtra);
    }
}
