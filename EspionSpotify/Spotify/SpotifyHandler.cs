using EspionSpotify.AudioSessions;
using EspionSpotify.Events;
using EspionSpotify.Models;
using System;
using System.Timers;

namespace EspionSpotify.Spotify
{
    public class SpotifyHandler: ISpotifyHandler, IDisposable
    {
        private const int TIMER_INTERVAL = 50;

        public Timer EventTimer { get; private set; }
        public Timer SongTimer { get; private set; }
        public ISpotifyProcess SpotifyProcess { get; private set; }
        public ISpotifyStatus SpotifyLatestStatus { get; private set; }

        private bool _listenForEvents;
        public bool ListenForEvents
        {
            get
            {
                return _listenForEvents;
            }
            set
            {
                _listenForEvents = value;
                EventTimer.Enabled = value;
            }
        }

        public Track Track { get; set; }

        public SpotifyHandler(ISpotifyAudioSession spotifyAudioSession)
        {
            SpotifyProcess = new SpotifyProcess(spotifyAudioSession);
            AttachTimer(TIMER_INTERVAL);
        }
        
        public SpotifyHandler(ISpotifyProcess spotifyProcess)
        {
            SpotifyProcess = spotifyProcess;
            AttachTimer(TIMER_INTERVAL);
        }

        public event EventHandler<TrackChangeEventArgs> OnTrackChange;

        public event EventHandler<PlayStateEventArgs> OnPlayStateChange;

        public event EventHandler<TrackTimeChangeEventArgs> OnTrackTimeChange;

        public Track GetTrack()
        {
            if (SpotifyLatestStatus == null)
            {
                return SpotifyProcess.GetSpotifyStatus()?.CurrentTrack;
            }

            return SpotifyLatestStatus.GetTrack();
        }

        private void ElapsedEventTick(object sender, ElapsedEventArgs e)
        {
            SpotifyLatestStatus = SpotifyProcess.GetSpotifyStatus();
            if (SpotifyLatestStatus?.CurrentTrack == null)
            {
                EventTimer.Start();
                return;
            }

            var newestTrack = SpotifyLatestStatus.CurrentTrack;
            if (Track != null)
            {
                if (newestTrack.Playing != Track.Playing)
                {
                    if (newestTrack.Playing)
                    {
                        SongTimer.Start();
                    }
                    else
                    {
                        SongTimer.Stop();
                    }

                    OnPlayStateChange?.Invoke(this, new PlayStateEventArgs()
                    {
                        Playing = newestTrack.Playing
                    });
                }
                if (!newestTrack.Equals(Track))
                {
                    SongTimer.Start();
                    OnTrackChange?.Invoke(this, new TrackChangeEventArgs()
                    {
                        OldTrack = Track,
                        NewTrack = SpotifyLatestStatus.GetTrack()
                    });
                }
                if (Track.CurrentPosition != null)
                {
                    OnTrackTimeChange?.Invoke(this, new TrackTimeChangeEventArgs()
                    {
                        TrackTime = Track.CurrentPosition ?? 0
                    });
                }
            }
            if (newestTrack != null)
            {
                newestTrack.CurrentPosition = newestTrack.Equals(Track) ? Track?.CurrentPosition ?? 0 : (int?)null;
                Track = newestTrack;
            }
            EventTimer.Start();
        }

        private void ElapsedSongTick(object sender, ElapsedEventArgs e)
        {
            if (Track == null) return;

            Track.CurrentPosition++;
        }

        private void AttachTimer(int interval)
        {
            EventTimer = new Timer
            {
                Interval = interval,
                AutoReset = false,
                Enabled = false
            };
            SongTimer = new Timer
            {
                Interval = interval * 20,
                AutoReset = true,
                Enabled = false
            };
            EventTimer.Elapsed += ElapsedEventTick;
            SongTimer.Elapsed += ElapsedSongTick;
        }

        public void Dispose()
        {
            if (EventTimer != null)
            {
                EventTimer.Enabled = false;
                EventTimer.Elapsed -= ElapsedEventTick;
                EventTimer.Dispose();
            }

            if (SongTimer != null)
            {
                SongTimer.Enabled = false;
                SongTimer.Elapsed -= ElapsedSongTick;
                SongTimer.Dispose();
            }
        }
    }
}
