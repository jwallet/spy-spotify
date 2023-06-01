using System;
using System.Threading.Tasks;
using System.Timers;
using EspionSpotify.AudioSessions;
using EspionSpotify.Events;
using EspionSpotify.Extensions;
using EspionSpotify.Models;

namespace EspionSpotify.Spotify
{
    public sealed class SpotifyHandler : ISpotifyHandler, IDisposable
    {
        private const int EVENT_TIMER_INTERVAL = 70;
        private const int SONG_TIMER_INTERVAL = 1000;
        private bool _disposed;

        private bool _listenForEvents;
        private bool _processingEvents;

        public SpotifyHandler(IMainAudioSession audioSession) : this(
            new SpotifyProcess(audioSession)
        )
        {
        }

        public SpotifyHandler(ISpotifyProcess spotifyProcess)
        {
            SpotifyProcess = spotifyProcess;
            EventTimer = new Timer();
            SongTimer = new Timer();
            AttachTimerToTickEvent();
        }

        public Timer EventTimer { get; private set; }
        public Timer SongTimer { get; private set; }
        public ISpotifyProcess SpotifyProcess { get; }
        public ISpotifyStatus SpotifyLatestStatus { get; private set; }

        public bool ListenForEvents
        {
            get => _listenForEvents;
            set
            {
                _listenForEvents = value;
                if (_listenForEvents) Track = new Track();
                EventTimerEnabled(value);
            }
        }

        public Track Track { get; set; }

        public event EventHandler<TrackChangeEventArgs> OnTrackChange;

        public event EventHandler<PlayStateEventArgs> OnPlayStateChange;

        public event EventHandler<TrackTimeChangeEventArgs> OnTrackTimeChange;

        public async Task<Track> GetTrack()
        {
            if (SpotifyLatestStatus == null) return (await SpotifyProcess.GetSpotifyStatus())?.CurrentTrack;

            return await SpotifyLatestStatus.GetTrack();
        }

        public async Task TriggerEvents()
        {
            // avoid concurrences
            if (_processingEvents) return;

            _processingEvents = true;

            SpotifyLatestStatus = await SpotifyProcess.GetSpotifyStatus();

            if (SpotifyLatestStatus?.CurrentTrack != null)
            {
                var newestTrack = SpotifyLatestStatus.CurrentTrack;
                if (Track != null)
                {
                    if (newestTrack.Playing != Track.Playing)
                    {
                        if (newestTrack.Playing)
                            SongTimer?.Start();
                        else
                            SongTimer?.Stop();

                        _ = Task.Run(() => OnPlayStateChange?.Invoke(this, new PlayStateEventArgs
                        {
                            Playing = newestTrack.Playing
                        }));
                    }

                    if (!newestTrack.Equals(Track))
                    {
                        var oldTrack = new Track(Track);
                        SongTimer?.Start();
                        _ = Task.Run(async () => OnTrackChange?.Invoke(this, new TrackChangeEventArgs
                        {
                            OldTrack = oldTrack,
                            NewTrack = await SpotifyLatestStatus.GetTrack()
                        }));
                    }

                    if (Track.CurrentPosition != null || newestTrack != null)
                        _ = Task.Run(() => OnTrackTimeChange?.Invoke(this, new TrackTimeChangeEventArgs
                        {
                            TrackTime = newestTrack.Equals(Track) ? Track?.CurrentPosition ?? 0 : 0
                        }));
                }

                if (newestTrack != null)
                {
                    if (!newestTrack.Equals(Track))
                    {
                        Track = newestTrack;
                    }
                }
            }

            EventTimerStart();

            _processingEvents = false;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private async void ElapsedEventTick(object sender, ElapsedEventArgs e)
        {
            await Task.Run(TriggerEvents);
        }

        private void EventTimerStart()
        {
            if (EventTimer == null) return;
            try
            {
                EventTimer.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void EventTimerEnabled(bool value)
        {
            if (EventTimer == null) return;
            try
            {
                EventTimer.Enabled = value;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ElapsedSongTick(object sender, ElapsedEventArgs e)
        {
            if (Track == null) return;

            Track.CurrentPosition++;
        }

        private void AttachTimerToTickEvent()
        {
            EventTimer.Interval = EVENT_TIMER_INTERVAL;
            EventTimer.AutoReset = false;
            EventTimer.Enabled = false;
            EventTimer.Elapsed += ElapsedEventTick;

            SongTimer.Interval = SONG_TIMER_INTERVAL;
            SongTimer.AutoReset = true;
            SongTimer.Enabled = false;
            SongTimer.Elapsed += ElapsedSongTick;
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (EventTimer != null)
                {
                    EventTimer.Stop();
                    EventTimer.Elapsed -= ElapsedEventTick;

                    EventTimer.Dispose();
                    EventTimer = null;
                }

                if (SongTimer != null)
                {
                    SongTimer.Stop();
                    SongTimer.Elapsed -= ElapsedSongTick;

                    SongTimer.Dispose();
                    SongTimer = null;
                }
            }

            _disposed = true;
        }
    }
}