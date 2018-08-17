using EspionSpotify.Events;
using EspionSpotify.Models;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace EspionSpotify.Spotify
{
    public interface ISpotifyHandler
    {
        Timer EventTimer { get; }
        Timer SongTimer { get; }
        ISpotifyProcess SpotifyProcess { get;}
        ISpotifyStatus SpotifyLatestStatus  { get;}
        bool ListenForEvents { get; set; }

        Track Track { get; set; }

        event EventHandler<TrackChangeEventArgs> OnTrackChange;
        event EventHandler<PlayStateEventArgs> OnPlayStateChange;
        event EventHandler<TrackTimeChangeEventArgs> OnTrackTimeChange;

        Track GetTrack();

        void Dispose();
    }
}
