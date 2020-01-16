using EspionSpotify.AudioSessions;
using EspionSpotify.Events;
using EspionSpotify.Models;
using System;
using System.Threading.Tasks;
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

        public SpotifyHandler(ISpotifyAudioSession spotifyAudioSession): this(
            spotifyProcess: new SpotifyProcess(spotifyAudioSession)
        ) {}

        public SpotifyHandler(ISpotifyProcess spotifyProcess)
        {
            SpotifyProcess = spotifyProcess;
            EventTimer = new Timer();
            SongTimer = new Timer();
            AttachTimerToTickEvent();
        }

        public event EventHandler<TrackChangeEventArgs> OnTrackChange;

        public event EventHandler<PlayStateEventArgs> OnPlayStateChange;

        public event EventHandler<TrackTimeChangeEventArgs> OnTrackTimeChange;

        public async Task<Track> GetTrack()
        {
            if (SpotifyLatestStatus == null)
            {
                return (await SpotifyProcess.GetSpotifyStatus())?.CurrentTrack;
            }

            return await SpotifyLatestStatus.GetTrack();
        }

        private async void ElapsedEventTick(object sender, ElapsedEventArgs e)
        {
            SpotifyLatestStatus = await SpotifyProcess.GetSpotifyStatus();
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

                    await Task.Run(() => OnPlayStateChange?.Invoke(this, new PlayStateEventArgs()
                    {
                        Playing = newestTrack.Playing
                    }));
                }
                if (!newestTrack.Equals(Track))
                {
                    SongTimer.Start();
                    await Task.Run(async () => OnTrackChange?.Invoke(this, new TrackChangeEventArgs()
                    {
                        OldTrack = Track,
                        NewTrack = await SpotifyLatestStatus.GetTrack()
                    }));
                }
                if (Track.CurrentPosition != null || newestTrack != null)
                {
                    await Task.Run(() => OnTrackTimeChange?.Invoke(this, new TrackTimeChangeEventArgs()
                    {
                        TrackTime = newestTrack.Equals(Track) ? Track?.CurrentPosition ?? 0 : 0
                    }));
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

        private void AttachTimerToTickEvent()
        {
            EventTimer.Interval = TIMER_INTERVAL;
            EventTimer.AutoReset = false;
            EventTimer.Enabled = false;
            EventTimer.Elapsed += ElapsedEventTick;

            SongTimer.Interval = TIMER_INTERVAL * 20;
            SongTimer.AutoReset = true;
            SongTimer.Enabled = false;
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
