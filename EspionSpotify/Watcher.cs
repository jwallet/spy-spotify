using EspionSpotify.AudioSessions;
using EspionSpotify.Events;
using EspionSpotify.Extensions;
using EspionSpotify.MediaTags;
using EspionSpotify.Models;
using EspionSpotify.Native;
using EspionSpotify.Properties;
using EspionSpotify.Spotify;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace EspionSpotify
{
    public class Watcher : IWatcher, IDisposable
    {
        private bool _disposed = false;
        private const bool MUTE = true;
        private const int NEXT_SONG_EVENT_MAX_ESTIMATED_DELAY = 5;

        public static bool Running { get; internal set; }
        public static bool Ready { get; private set; } = true;
        public static bool ToggleStopRecordingDelayed { get; internal set; }

        private IRecorder _recorder;
        private readonly IMainAudioSession _audioSession;
        private Timer _recordingTimer;
        private bool _isPlaying;
        private Track _currentTrack;
        private bool _stopRecordingWhenSongEnds;
        private readonly IFileSystem _fileSystem;

        private readonly IFrmEspionSpotify _form;
        private readonly UserSettings _userSettings;
        private readonly List<RecorderTask> _recorderTasks = new List<RecorderTask>();

        public int CountSeconds { get; set; }
        public ISpotifyHandler Spotify { get; set; }

        public bool RecorderUpAndRunning
        {
            get => _recorder != null && _recorder.Running;
        }
        public bool IsRecordUnknownActive
        {
            get => _userSettings.RecordEverythingEnabled && (_currentTrack.IsUnknown || _userSettings.RecordAdsEnabled);
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

        internal Watcher(IFrmEspionSpotify form, IMainAudioSession audioSession, UserSettings userSettings) :
            this(form, audioSession, userSettings, track: new Track(), fileSystem: new FileSystem())
        { }

        public Watcher(IFrmEspionSpotify form, IMainAudioSession audioSession, UserSettings userSettings, Track track, IFileSystem fileSystem, IRecorder recorder = null)
        {
            _form = form;
            _audioSession = audioSession;
            _userSettings = userSettings;
            _currentTrack = track;
            _fileSystem = fileSystem;
            _recorder = recorder;

            Settings.Default.Logs = string.Empty;
            Settings.Default.Save();
        }

        private void OnPlayStateChanged(object sender, PlayStateEventArgs e)
        {
            // it will be triggered after onTrackChanged from track Spotify playing (fading out) to track Spotify paused

            if (e.Playing == _isPlaying) return;
            _isPlaying = e.Playing;

            // was paused
            if (!_isPlaying && _recorder != null)
            {
                _form.UpdateNumUp();
            }

            _form.UpdateIconSpotify(_isPlaying);
        }

        public void OnTrackChanged(object sender, TrackChangeEventArgs e)
        {
            // do not add "is current track an ad" validation, audio is already muted
            if (RecorderUpAndRunning && IsOldSong)
            {
                _audioSession.SleepWhileTheSongEnds();
            }

            if (!IsNewTrack(e.NewTrack)) return;

            DoIKeepLastSong();
            StopLastRecorder();

            if (IsMaxOrderNumberAsFileExceeded)
            {
                _form.WriteIntoConsole(I18nKeys.LogMaxFileSequenceReached, _userSettings.OrderNumberMax);
            }

            if (!_isPlaying || RecorderUpAndRunning || !IsTypeAllowed || IsMaxOrderNumberAsFileExceeded) return;

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

            var adTitle = !IsRecordUnknownActive && _currentTrack.Ad && !SpotifyStatus.WindowTitleIsSpotify(_currentTrack.ToString()) ? $"{_form.Rm?.GetString(I18nKeys.LogAd) ?? "Ad"}: " : "";
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
            return await _audioSession.WaitSpotifyAudioSessionToStart(Running);
        }

        private void BindSpotifyEventHandlers()
        {
            Spotify = new SpotifyHandler(_audioSession);

            Spotify.OnPlayStateChange += OnPlayStateChanged;
            Spotify.OnTrackChange += OnTrackChanged;
            Spotify.OnTrackTimeChange += OnTrackTimeChanged;
        }

        public async Task Run()
        {
            if (Running) return;

            _form.WriteIntoConsole(I18nKeys.LogStarting);

            await RunSpotifyConnect();
            var isAudioSessionNotFound = !await SetSpotifyAudioSessionAndWaitToStart();
            BindSpotifyEventHandlers();
            Ready = false;

            if (!Recorder.TestFileWriter(_form, _audioSession, _userSettings))
            {
                EndRecordingSession();
            }

            if (isAudioSessionNotFound)
            {
                _form.WriteIntoConsole(I18nKeys.LogSpotifyPlayingOutsideOfSelectedAudioEndPoint);
                Running = false;
            }
            else if (SpotifyConnect.IsSpotifyRunning())
            {
                await InitializeRecordingSession();
                NativeMethods.PreventSleep();

                while (Running)
                {
                    // Order is important
                    if (!SpotifyConnect.IsSpotifyRunning())
                    {
                        _form.WriteIntoConsole(I18nKeys.LogSpotifyIsClosed);
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
                    await Task.Delay(500);
                }

                DoIKeepLastSong();
                StopLastRecorder();
                NativeMethods.AllowSleep();
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
            if (!ExternalAPI.Instance.IsAuthenticated) return;
            if (_stopRecordingWhenSongEnds)
            {
                Running = false;
                _stopRecordingWhenSongEnds = false;
                return;
            }

            ManageRecorderTasks();
            CountSeconds = 0;

            _recorder = new Recorder(_form, _audioSession, _userSettings, _currentTrack, _fileSystem);
            var token = new CancellationTokenSource();
            _recorderTasks.Add(new RecorderTask() { Task = Task.Run(() => _recorder.Run(token)), Token = token });

            _form.UpdateIconSpotify(_isPlaying, true);
        }

        private async Task InitializeRecordingSession()
        {
            _audioSession.SetSpotifyVolumeToHighAndOthersToMute(MUTE);

            var track = await Spotify.GetTrack();

            if (track == null) return;

            _isPlaying = track.Playing;
            _form.UpdateIconSpotify(_isPlaying);

            Spotify.Track = new Track();
            Spotify.ListenForEvents = true;

            _form.UpdatePlayingTitle(track.ToString());
            MutesSpotifyAds(track.Ad);

            if (_userSettings.HasRecordingTimerEnabled)
            {
                await Task.Run(EnableRecordingTimer);
            }
        }

        private async Task EnableRecordingTimer()
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

            if (_audioSession != null)
            {
                MutesSpotifyAds(false);
                _audioSession.SetSpotifyVolumeToHighAndOthersToMute(false);
                _audioSession.ClearSpotifyAudioSessionControls();

                Spotify.ListenForEvents = false;
                Spotify.OnPlayStateChange -= OnPlayStateChanged;
                Spotify.OnTrackChange -= OnTrackChanged;
                Spotify.OnTrackTimeChange -= OnTrackTimeChanged;
                Spotify.Dispose();
            }

            _form.UpdateStartButton();
            _form.UpdatePlayingTitle(Constants.SPOTIFY);
            _form.UpdateIconSpotify(false);
            _form.UpdateRecordedTime(null);
            _form.StopRecording();
        }

        private void ManageRecorderTasks()
        {
            if (_recorderTasks.Count > 0)
            {
                _form.UpdateNumUp();
            }
            _recorderTasks.RemoveAll(x => x.Task.Status == TaskStatus.RanToCompletion);
        }

        private void DoIKeepLastSong()
        {
            // always increment when session ends
            if (!Running && _recorderTasks.Any(t => t.Task.Status != TaskStatus.RanToCompletion))
            {
                _form.UpdateNumUp();
            }
            // valid if the track is removed, go back one count
            if (RecorderUpAndRunning && CountSeconds < _userSettings.MinimumRecordedLengthSeconds)
            {
                _form.UpdateNumDown();
            }
        }

        private void StopLastRecorder()
        {
            if (_recorder == null) return;

            _recorder.Running = false;
            _recorder.CountSeconds = CountSeconds;
            _form.UpdateIconSpotify(_isPlaying);
        }

        private void MutesSpotifyAds(bool value)
        {
            if (_userSettings.MuteAdsEnabled || !value)
            {
                _audioSession.SetSpotifyToMute(value);
            }
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                DoIKeepLastSong();
                NativeMethods.AllowSleep();

                if (_audioSession != null)
                {
                    MutesSpotifyAds(false);
                    _audioSession.SetSpotifyVolumeToHighAndOthersToMute(false);
                    _audioSession.ClearSpotifyAudioSessionControls();

                    Spotify.ListenForEvents = false;
                    Spotify.OnPlayStateChange -= OnPlayStateChanged;
                    Spotify.OnTrackChange -= OnTrackChanged;
                    Spotify.OnTrackTimeChange -= OnTrackTimeChanged;
                    Spotify.Dispose();
                }

                _recorderTasks.ForEach(x =>
                {
                    x.Token.Cancel();
                    x.Task.Wait();
                    x.Task.Dispose();
                });

                if (_recorder != null) _recorder.Dispose();
            }

            _disposed = true;
        }
    }

}
