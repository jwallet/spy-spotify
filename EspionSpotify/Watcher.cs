using EspionSpotify.Events;
using EspionSpotify.Models;
using EspionSpotify.Properties;
using EspionSpotify.Spotify;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace EspionSpotify
{
    public class Watcher : IWatcher
    {
        private const string SPOTIFY = "Spotify";
        private const bool MUTE = true;
        private const int NEXT_SONG_EVENT_MAX_ESTIMATED_DELAY = 5;

        private static bool _running;

        public static bool Running { get => _running; internal set { _running = value; } }
        public static bool Ready { get; private set; } = true;
        public static bool ToggleStopRecordingDelayed { get; internal set; }

        private IRecorder _recorder;
        private Timer _recordingTimer;
        private bool _isPlaying;
        private Track _currentTrack;
        private bool _stopRecordingWhenSongEnds;

        private readonly IFrmEspionSpotify _form;
        private readonly UserSettings _userSettings;

        public int CountSeconds { get; set; }
        public ISpotifyHandler Spotify { get; set; }

        public bool RecorderUpAndRunning { get => _recorder != null && _recorder.Running; }
        public bool NumTrackActivated { get => _userSettings.OrderNumber.HasValue; }
        public bool AdPlaying { get => _currentTrack.Ad; }
        public string SongTitle { get => _currentTrack.ToString(); }
        public bool IsRecordUnknownActive { get => _userSettings.RecordUnknownTrackTypeEnabled && _currentTrack.Playing && !SpotifyStatus.WindowTitleIsSpotify(_currentTrack.ToString()); }
        public bool IsTrackExists { get => !_userSettings.DuplicateAlreadyRecordedTrack && FileManager.IsPathFileNameExists(_currentTrack, _userSettings);  }
        public bool IsTypeAllowed
        {
            get => _currentTrack.IsNormal() || IsRecordUnknownActive;
        }
        public bool IsOldSong
        { 
            get => _userSettings.EndingTrackDelayEnabled && _currentTrack.Length > 0 && _currentTrack.CurrentPosition > _currentTrack.Length - NEXT_SONG_EVENT_MAX_ESTIMATED_DELAY;
        }

        public Watcher(IFrmEspionSpotify form, UserSettings userSettings)
        {
            _currentTrack = new Track();
            _form = form;
            _userSettings = userSettings;
            _userSettings.InternalOrderNumber--;

            Settings.Default.Logs = string.Empty;
            Settings.Default.Save();
        }

        private void OnPlayStateChanged(object sender, PlayStateEventArgs e)
        {
            if (e.Playing == _isPlaying) return;
            _isPlaying = e.Playing;

            _form.UpdateIconSpotify(_isPlaying);
        }

        public void OnTrackChanged(object sender, TrackChangeEventArgs e)
        {
            // do not add "is current track an ad" validation, audio is already muted
            if (RecorderUpAndRunning && IsOldSong)
            {
                _userSettings.SpotifyAudioSession.SleepWhileTheSongEnds();
            }

            if (!IsNewTrack(e.NewTrack)) return;

            DoIKeepLastSong();

            if (IsTrackExists)
            {
                _form.WriteIntoConsole("logTrackExists", _currentTrack.ToString());
            }

            if (!_isPlaying || RecorderUpAndRunning || !IsTypeAllowed || IsTrackExists) return;

            RecordSpotify();
        }

        private void OnTrackTimeChanged(object sender, TrackTimeChangeEventArgs e)
        {
            _currentTrack.CurrentPosition = e.TrackTime;
            _form.UpdateRecordedTime(RecorderUpAndRunning ? (int?)e.TrackTime : null);
        }

        public bool IsNewTrack(Track track)
        {
            if (track == null) return false;

            if (_currentTrack.Equals(track))
            {
                _form.UpdateIconSpotify(_isPlaying, RecorderUpAndRunning);
                return false;
            }

            _currentTrack = track;
            _isPlaying = _currentTrack.Playing;

            var adTitle = _currentTrack.Ad && !SpotifyStatus.WindowTitleIsSpotify(_currentTrack.ToString()) ? $"{_form.Rm?.GetString($"logAd") ?? "Ad"}: " : "";
            _form.UpdatePlayingTitle($"{adTitle}{SongTitle}");

            MutesSpotifyAds(AdPlaying);

            return true;
        }

        private async Task RunSpotifyConnect()
        {
            if (!SpotifyConnect.IsSpotifyInstalled()) return;

            if (!SpotifyConnect.IsSpotifyRunning())
            {
                _form.WriteIntoConsole("logSpotifyConnecting");
                await SpotifyConnect.Run();
            }

            Running = true;
        }

        private async Task<bool> SetSpotifyAudioSessionAndWaitToStart()
        {
            _userSettings.SpotifyAudioSession = new AudioSessions.SpotifyAudioSession(_userSettings.AudioEndPointDeviceIndex);
            return await _userSettings.SpotifyAudioSession.WaitSpotifyAudioSessionToStart(_running);
        }

        private void BindSpotifyEventHandlers()
        {
            Spotify = new SpotifyHandler(_userSettings.SpotifyAudioSession)
            {
                ListenForEvents = true
            };
            Spotify.OnPlayStateChange += OnPlayStateChanged;
            Spotify.OnTrackChange += OnTrackChanged;
            Spotify.OnTrackTimeChange += OnTrackTimeChanged;
        }

        public async Task Run()
        {
            if (Running) return;

            _form.WriteIntoConsole($"logStarting");

            await RunSpotifyConnect();
            var isAudioSessionNotFound = !(await SetSpotifyAudioSessionAndWaitToStart());
            BindSpotifyEventHandlers();
            Ready = false;

            if (SpotifyConnect.IsSpotifyRunning())
            {
                _currentTrack = await Spotify.GetTrack();
                InitializeRecordingSession();

                while (Running)
                {
                    // Order is important
                    if (!SpotifyConnect.IsSpotifyRunning())
                    {
                        _form.WriteIntoConsole("logSpotifyIsClosed");
                        Running = false;
                    }
                    else if (isAudioSessionNotFound)
                    {
                        _form.WriteIntoConsole("logSpotifyPlayingOutsideOfSelectedAudioEndPoint");
                        Running = false;
                    }
                    else if (ToggleStopRecordingDelayed)
                    {
                        ToggleStopRecordingDelayed = false;
                        _stopRecordingWhenSongEnds = true;
                        _form.WriteIntoConsole("logStopRecordingWhenSongEnds");
                    }
                    else if (!_stopRecordingWhenSongEnds && _userSettings.HasRecordingTimerEnabled && !_recordingTimer.Enabled)
                    {
                        _form.WriteIntoConsole("logRecordingTimerDone");
                        ToggleStopRecordingDelayed = true;
                    }
                    await Task.Delay(200);
                }

                DoIKeepLastSong();
            }
            else if (SpotifyConnect.IsSpotifyInstalled())
            {
                _form.WriteIntoConsole(isAudioSessionNotFound ? "logSpotifyIsClosed" : "logSpotifyNotConnected");
            }
            else
            {
                _form.WriteIntoConsole("logSpotifyNotFound");
            }

            EndRecordingSession();

            _form.WriteIntoConsole("logStoping");
        }

        private void RecordSpotify()
        {
            if (_stopRecordingWhenSongEnds)
            {
                Running = false;
                _stopRecordingWhenSongEnds = false;
                return;
            }

            _recorder = new Recorder(_form, _userSettings, _currentTrack);

            var recorderTask = new Task(_recorder.Run);
            recorderTask.Start();

            _form.UpdateIconSpotify(_isPlaying, true);
            UpdateNumUp();
            CountSeconds = 0;
        }

        private async void InitializeRecordingSession()
        {
            _userSettings.SpotifyAudioSession.SetSpotifyVolumeToHighAndOthersToMute(MUTE);

            var track = await Spotify.GetTrack();
            if (track == null) return;

            _isPlaying = track.Playing;
            _form.UpdateIconSpotify(_isPlaying);

            _currentTrack = track;

            _form.UpdatePlayingTitle(SongTitle);
            MutesSpotifyAds(AdPlaying);

            if (_userSettings.HasRecordingTimerEnabled)
            {
                EnableRecordingTimer();
            }
        }

        private async void EnableRecordingTimer()
        {
            _recordingTimer = new Timer(_userSettings.RecordingTimerMilliseconds)
            {
                AutoReset = false,
                Enabled = false
            };

            while (_recorder == null && SpotifyConnect.IsSpotifyRunning())
            {
                await Task.Delay(300);
            }

            _recordingTimer.Enabled = true;
        }

        private void EndRecordingSession()
        {
            Ready = true;

            if (_userSettings.SpotifyAudioSession != null)
            {
                MutesSpotifyAds(false);
                _userSettings.SpotifyAudioSession.SetSpotifyVolumeToHighAndOthersToMute(false);

                Spotify.ListenForEvents = false;
                Spotify.OnPlayStateChange -= OnPlayStateChanged;
                Spotify.OnTrackChange -= OnTrackChanged;
                Spotify.OnTrackTimeChange -= OnTrackTimeChanged;
            }

            _form.UpdateStartButton();
            _form.UpdatePlayingTitle(SPOTIFY);
            _form.UpdateIconSpotify(false);
            _form.UpdateRecordedTime(null);
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
