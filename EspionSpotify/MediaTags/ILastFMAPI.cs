using EspionSpotify.Models;

namespace EspionSpotify.MediaTags
{
    public interface ILastFMAPI
    {
        LastFMTrack TrackInfo { get; set; }
        LastFMTrack GetTagInfo(Track track);
    }
}