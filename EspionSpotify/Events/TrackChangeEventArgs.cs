using EspionSpotify.Models;

namespace EspionSpotify.Events
{
    /// <summary>
    ///     Event gets triggered, when the Track is changed
    /// </summary>
    public class TrackChangeEventArgs
    {
        public Track OldTrack { get; set; }
        public Track NewTrack { get; set; }
    }
}