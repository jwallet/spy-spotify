using EspionSpotify.Models;

namespace EspionSpotify.API
{
    public interface ILastFMAPI
    {
        string[] ApiKeys { get; }

        void MapLastFMTrackToTrack(Track track, LastFMTrack trackExtra);
    }
}