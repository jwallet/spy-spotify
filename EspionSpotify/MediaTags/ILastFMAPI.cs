using EspionSpotify.Models;
using System.Threading.Tasks;

namespace EspionSpotify.MediaTags
{
    public interface ILastFMAPI
    {
        string[] ApiKeys { get; }

        void MapLastFMTrackToTrack(Track track, LastFMTrack trackExtra);
    }
}
