using EspionSpotify.Events;
using EspionSpotify.Models;
using EspionSpotify.Properties;
using EspionSpotify.Spotify;
using System.IO.Abstractions;
using EspionSpotify.Extensions;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;
using EspionSpotify.AudioSessions;
using System;

namespace EspionSpotify
{
    public class Watcher : IWatcher
    {
        private const string SPOTIFY = "Spotify";
        private const bool MUTE = true;
        private const int NEXT_SONG_EVENT_MAX_ESTIMATED_DELAY = 5;

        public static bool Running { get; internal set; }
        public static bool Ready { get; private set; } = true;
        public static bool ToggleStopRecordingDelayed { get; internal set; }

        private IRecorder _recorder;
        private Timer _recordingTimer;
        private bool _isPlaying;
        private Track _currentTrack;
        private bool _stopRecordingWhenSongEnds;
        private readonly IFileSystem _fileSystem;

        private readonly IFrmEspionSpotify _form;
        private readonly UserSettings _userSettings;

        public int CountSeconds { get; set; }
        public ISpotifyHandler Spotify { get; set; }

        public bool RecorderUpAndRunning
        {
            get => _recorder != null && _recorder.Running;
        }
        public bool IsRecordUnknownActive
        {
            get => _userSettings.RecordUnknownTrackTypeEnabled
                && !SpotifyStatus.WindowTitleIsSpotify(_currentTrack.ToString());
        }
        public bool IsSkipTrackActive
        {
            get => _userSettings.RecordRecordingsStatus == Enums.RecordRecordingsStatus.Skip
                && FileManager.IsPathFileNameExists(_currentTrack, _userSettings, _fileSystem);
        }
        public bool IsTypeAllowed
        {
            get => _currentTrack.IsNormal || IsRecordUnknownActive;
        }
        public bool IsOldSong
        { 
            get => _userSettings.EndingTrackDelayEnabled && _currentTrack.Length > 0 
                && _currentTrack.CurrentPosition > Math.Max(0, (_currentTrack.Length ?? 0) - NEXT_SONG_EVENT_MAX_ESTIMATED_DELAY);
        }
        public bool IsMaxOrderNumberAsFileExceeded
        {
            get => _userSettings.OrderNumberInfrontOfFileEnabled && _userSettings.OrderNumberAsFile == _userSettings.OrderNumberMax;
        }

        internal Watcher(IFrmEspionSpotify form, UserSettings userSettings):
            this(form, userSettings, track: new Track(), fileSystem: new FileSystem()) {}

        public Watcher(IFrmEspionSpotify form, UserSettings userSettings, Track track, IFileSystem fileSystem, IRecorder recorder = null)
        {
            _form = form;
            _userSettings = userSettings;
            _currentTrack = track;
            _fileSystem = fileSystem;
            _recorder = recorder;

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
                _userSettings.AudioSession.SleepWhileTheSongEnds();
            }

            if (!IsNewTrack(e.NewTrack)) return;

            DoIKeepLastSong();

            if (IsSkipTrackActive)
            {
                _form.WriteIntoConsole(I18nKeys.LogTrackExists, _currentTrack.ToString());
            }

            if (IsMaxOrderNumberAsFileExceeded)
            {
                _form.WriteIntoConsole(I18nKeys.LogMaxFileSequenceReached, _userSettings.OrderNumberMax);
            }

            if (!_isPlaying || RecorderUpAndRunning || !IsTypeAllowed || IsSkipTrackActive || IsMaxOrderNumberAsFileExceeded) return;

            RecordSpotify();
        }

        private void OnTrackTimeChanged(object sender, TrackTimeChangeEventArgs e)
        {
            _currentTrack.CurrentPosition = e.TrackTime;
            _form.UpdateRecordedTime(RecorderUpAndRunning ? (int?)e.TrackTime : null);
        }

        public bool IsNewTrack(Track track)
        {
            if (track == null || new Track().Equals(track)) return false;

            if (_currentTrack.Equals(track))
            {
                _form.UpdateIconSpotify(_isPlaying, RecorderUpAndRunning);
                return false;
            }

            _currentTrack = track;
            _isPlaying = _currentTrack.Playing;

            var adTitle = _currentTrack.Ad && !SpotifyStatus.WindowTitleIsSpotify(_currentTrack.ToString()) ? $"{_form.Rm?.GetString(I18nKeys.LogAd) ?? "Ad"}: " : "";
            _form.UpdatePlayingTitle($"{adTitle}{_currentTrack.ToString()}");

            MutesSpotifyAds(_currentTrack.Ad);

            return true;
        }

        private async Task RunSpotifyConnect()
        {
            if (!SpotifyConnect.IsSpotifyInstalled(_fileSystem)) return;

            if (!SpotifyConnect.IsSpotifyRunning())
            {
                _form.WriteIntoConsole(I18nKeys.LogSpotifyConnecting);
                await SpotifyConnect.Run(_fileSystem);
            }

            Running = true;
        }

        private async Task<bool> SetSpotifyAudioSessionAndWaitToStart()
        {
            return await _userSettings.AudioSession.WaitSpotifyAudioSessionToStart(Running);
        }

        private void BindSpotifyEventHandlers()
        {
            Spotify = new SpotifyHandler(_userSettings.AudioSession)
            {
                ListenForEvents = true
            };
            Spotify.OnPlayStateChange += OnPlayStateChanged;
            Spotify.OnTrackChange += OnTrackChanged;
            Spotify.OnTrackTimeChange += OnTrackTimeChanged;
        }

        public async void Run()
        {
            if (Running) return;

            _form.WriteIntoConsole(I18nKeys.LogStarting);

            await RunSpotifyConnect();
            var isAudioSessionNotFound = !await SetSpotifyAudioSessionAndWaitToStart();
            BindSpotifyEventHandlers();
            Ready = false;

            if (!Recorder.TestFileWriter(_form, _userSettings))
            {
                EndRecordingSession();
            }

            if (SpotifyConnect.IsSpotifyRunning())
            {
                _currentTrack = await Spotify.GetTrack();
                InitializeRecordingSession();

                while (Running)
                {
                    // Order is important
                    if (!SpotifyConnect.IsSpotifyRunning())
                    {
                        _form.WriteIntoConsole(I18nKeys.LogSpotifyIsClosed);
                        Running = false;
                    }
                    else if (isAudioSessionNotFound)
                    {
                        _form.WriteIntoConsole(I18nKeys.LogSpotifyPlayingOutsideOfSelectedAudioEndPoint);
                        Running = false;
                    }
                    else if (ToggleStopRecordingDelayed)
                    {
                        ToggleStopRecordingDelayed = false;
                        _stopRecordingWhenSongEnds = true;
                        _form.WriteIntoConsole(I18nKeys.LogStopRecordingWhenSongEnds);
                    }
                    else if (!_stopRecordingWhenSongEnds && _userSettings.HasRecordingTimerEnabled && !_recordingTimer.Enabled)
                    {
                        _form.WriteIntoConsole(I18nKeys.LogRecordingTimerDone);
                        ToggleStopRecordingDelayed = true;
                    }
                    await Task.Delay(200);
                }

                UpdateNumUp();
                DoIKeepLastSong();
            }
            else if (SpotifyConnect.IsSpotifyInstalled(_fileSystem))
            {
                _form.WriteIntoConsole(isAudioSessionNotFound ? I18nKeys.LogSpotifyIsClosed : I18nKeys.LogSpotifyNotConnected);
            }
            else
            {
                _form.WriteIntoConsole(I18nKeys.LogSpotifyNotFound);
            }

            EndRecordingSession();

            _form.WriteIntoConsole(I18nKeys.LogStoping);
        }

        private void RecordSpotify()
        {
            if (_stopRecordingWhenSongEnds)
            {
                Running = false;
                _stopRecordingWhenSongEnds = false;
                return;
            }

            _recorder = new Recorder(_form, _userSettings, _currentTrack, _fileSystem);

            Task.Run(_recorder.Run);

            // switched after recorder, look like first song in play status gets recorded when next track starts being recorded
            if (_recorder != null)
            {
                UpdateNumUp();
            }

            _form.UpdateIconSpotify(_isPlaying, true);
            CountSeconds = 0;
        }

        private async void InitializeRecordingSession()
        {
            _userSettings.AudioSession.SetSpotifyVolumeToHighAndOthersToMute(MUTE);

            var track = await Spotify.GetTrack();
            if (track == null) return;

            _isPlaying = track.Playing;
            _form.UpdateIconSpotify(_isPlaying);

            _currentTrack = track;

            _form.UpdatePlayingTitle(_currentTrack.ToString());
            MutesSpotifyAds(_currentTrack.Ad);

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

            if (_userSettings.AudioSession != null)
            {
                MutesSpotifyAds(false);
                _userSettings.AudioSession.SetSpotifyVolumeToHighAndOthersToMute(false);

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
            if (RecorderUpAndRunning)
            {
                UpdateNumDown();
            }

            if (_recorder != null)
            {
                _recorder.Running = false;
                _recorder.CountSeconds = CountSeconds;
                _form.UpdateIconSpotify(_isPlaying);
            }
        }

        private void UpdateNumDown()
        {
            if (!_userSettings.HasOrderNumberEnabled) return;

            if (CountSeconds < _userSettings.MinimumRecordedLengthSeconds)
            {
                _userSettings.InternalOrderNumber--;
            }

            _form.UpdateNum(_userSettings.InternalOrderNumber);
        }

        private void UpdateNumUp()
        {
            if (!_userSettings.HasOrderNumberEnabled) return;

            _userSettings.InternalOrderNumber++;
            _form.UpdateNum(_userSettings.InternalOrderNumber);
        }

        private void MutesSpotifyAds(bool value)
        {
            if (_userSettings.MuteAdsEnabled  && !_userSettings.RecordUnknownTrackTypeEnabled)
            {
                _userSettings.AudioSession.SetSpotifyToMute(value);
            }
        }
    }
}
