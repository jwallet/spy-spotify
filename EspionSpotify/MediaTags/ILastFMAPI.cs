using EspionSpotify.Models;

namespace EspionSpotify.MediaTags
{
    public interface ILastFMAPI
    {
        string[] ApiKeys { get; }

        void MapLastFMTrackToTrack(Track track, LastFMTrack trackExtra);
    }
}
