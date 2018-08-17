using EspionSpotify.Events;
using EspionSpotify.Models;
using EspionSpotify.Spotify;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EspionSpotify
{
    internal class Watcher: IWatcher
    {
        private const string _spotify = "Spotify";
        private const bool Mute = true;
        private const int NextSongEventMaxEstimatedDelay = 5;

        public static bool Running;
        public static bool Ready = true;

        private Recorder _recorder;
        private Track _currentTrack;
        private bool _isPlaying;

        private readonly FrmEspionSpotify _form;
        private readonly UserSettings _userSettings;

        public int CountSeconds { get; set; }
        public ISpotifyHandler Spotify { get; set; }

        private bool NumTrackActivated => _userSettings.OrderNumber.HasValue;
        private bool AdPlaying => _currentTrack.Ad;
        private string SongTitle => _currentTrack.ToString();
        private bool IsTypeAllowed => _currentTrack.IsNormal() || (_userSettings.RecordUnknownTrackTypeEnabled && _currentTrack.Playing);
        private bool RecorderUpAndRunning => _recorder != null && _recorder.Running;
        private bool IsOldSong => _userSettings.EndingTrackDelayEnabled && _currentTrack.Length > 0 && _currentTrack.CurrentPosition > _currentTrack.Length - NextSongEventMaxEstimatedDelay;

        public Watcher(FrmEspionSpotify form, UserSettings userSettings)
        {
            _form = form;
            _userSettings = userSettings;
        }

        private void OnPlayStateChanged(object sender, PlayStateEventArgs e)
        {
            if (e.Playing == _isPlaying) return;
            _isPlaying = e.Playing;

            _form.UpdateIconSpotify(_isPlaying);
        }

        private void OnTrackChanged(object sender, TrackChangeEventArgs e)
        {
            if ((RecorderUpAndRunning && IsOldSong) || e.OldTrack.Ad)
            {
                _userSettings.SpotifyAudioSession.SleepWhileTheSongEnds();
            }

            if (!IsNewTrack(e.NewTrack)) return;

            RecordSpotify();
        }

        private void OnTrackTimeChanged(object sender, TrackTimeChangeEventArgs e)
        {
            if (_currentTrack != null)
            {
                _currentTrack.CurrentPosition = e.TrackTime;
            }
        }

        private bool IsNewTrack(Track track)
        {
            if (track == null) return false;

            if (_currentTrack.Equals(track))
            {
                _form.UpdateIconSpotify(_isPlaying, RecorderUpAndRunning);
                return false;
            }

            _isPlaying = track.Playing;
            _currentTrack = track;

            _form.UpdatePlayingTitle(SongTitle);

            MutesSpotifyAds(AdPlaying);

            return true;
        }

        private async Task RunSpotifyConnect()
        {
            if (SpotifyConnect.IsSpotifyInstalled() && !SpotifyConnect.IsSpotifyRunning())
            {
                _form.WriteIntoConsole(FrmEspionSpotify.Rm.GetString($"logSpotifyConnecting"));
            }

            SpotifyConnect.Run();

            _userSettings.SpotifyAudioSession = new AudioSessions.SpotifyAudioSession();
            await _userSettings.SpotifyAudioSession.WaitSpotifyAudioSessionToStart();

            Spotify = new SpotifyHandler(_userSettings.SpotifyAudioSession)
            {
                ListenForEvents = true
            };
            Spotify.OnPlayStateChange += OnPlayStateChanged;
            Spotify.OnTrackChange += OnTrackChanged;
            Spotify.OnTrackTimeChange += OnTrackTimeChanged;

            _currentTrack = Spotify.GetTrack();
        }

        public async Task Run()
        {
            if (Running) return;

            Ready = false;
            Running = true;
            await RunSpotifyConnect();

            if (SpotifyConnect.IsSpotifyRunning())
            {
                InitializeRecordingSession();

                while (Running)
                {
                    if (!SpotifyConnect.IsSpotifyRunning())
                    {
                        _form.WriteIntoConsole(FrmEspionSpotify.Rm.GetString($"logSpotifyIsClosed"));
                        Running = false;
                    }
                    Thread.Sleep(200);
                }

                DoIKeepLastSong();
                _form.WriteIntoConsole(FrmEspionSpotify.Rm.GetString($"logStoping"));
            }
            else if (SpotifyConnect.IsSpotifyInstalled())
            {
                _form.WriteIntoConsole(FrmEspionSpotify.Rm.GetString($"logSpotifyNotConnected"));
            }
            else
            {
                _form.WriteIntoConsole(FrmEspionSpotify.Rm.GetString($"logSpotifyNotFound"));
            }

            EndRecordingSession();
        }

        private void RecordSpotify()
        {
            DoIKeepLastSong();

            if (!_isPlaying || RecorderUpAndRunning || !IsTypeAllowed) return;

            _recorder = new Recorder(_form, _userSettings, _currentTrack);

            var recorderThread = new Thread(_recorder.Run);
            recorderThread.Start();

            _form.UpdateIconSpotify(_isPlaying, true);
            UpdateNumUp();
            CountSeconds = 0;
        }

        private void InitializeRecordingSession()
        {
            _form.WriteIntoConsole(FrmEspionSpotify.Rm?.GetString($"logStarting") ?? "Starting");

            _userSettings.SpotifyAudioSession.SetSpotifyVolumeToHighAndOthersToMute(Mute);

            var track = Spotify.GetTrack();
            _isPlaying = track.Playing;
            _form.UpdateIconSpotify(_isPlaying);

            _currentTrack = track;

            _form.UpdatePlayingTitle(SongTitle);
            MutesSpotifyAds(AdPlaying);
        }

        private void EndRecordingSession()
        {
            if (Running)
            {
                Running = false;
            }

            Ready = true;

            MutesSpotifyAds(false);
            _userSettings.SpotifyAudioSession.SetSpotifyVolumeToHighAndOthersToMute(false);

            Spotify.ListenForEvents = false;
            Spotify.OnPlayStateChange -= OnPlayStateChanged;
            Spotify.OnTrackChange -= OnTrackChanged;
            Spotify.OnTrackTimeChange -= OnTrackTimeChanged;

            _form.UpdateStartButton();
            _form.UpdatePlayingTitle(_spotify);
            _form.UpdateIconSpotify(false);
            _form.StopRecording();
        }

        private void DoIKeepLastSong()
        {
            if (_recorder != null)
            {
                _recorder.Running = false;
                _recorder.CountSeconds = CountSeconds;
                _form.UpdateIconSpotify(_isPlaying);
            }

            if (RecorderUpAndRunning)
            {
                UpdateNumDown();
            }
        }

        private void UpdateNumDown()
        {
            if (!NumTrackActivated) return;

            if (CountSeconds < _userSettings.MinimumRecordedLengthSeconds)
            {
                _userSettings.InternalOrderNumber--;
            }

            _form.UpdateNum(_userSettings.OrderNumber.Value);
        }

        private void UpdateNumUp()
        {
            if (!NumTrackActivated) return;

            _userSettings.InternalOrderNumber++;
            _form.UpdateNum(_userSettings.OrderNumber.Value);
        }

        private void MutesSpotifyAds(bool value)
        {
            if (_userSettings.MuteAdsEnabled  && !_userSettings.RecordUnknownTrackTypeEnabled)
            {
                _userSettings.SpotifyAudioSession.SetSpotifyToMute(value);
            }
        }
    }
}
