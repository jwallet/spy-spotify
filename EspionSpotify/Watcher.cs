using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EspionSpotify.API;
using EspionSpotify.AudioSessions;
using EspionSpotify.Enums;
using EspionSpotify.Events;
using EspionSpotify.Extensions;
using EspionSpotify.Models;
using EspionSpotify.Native;
using EspionSpotify.Properties;
using EspionSpotify.Spotify;
using EspionSpotify.Translations;
using Timer = System.Timers.Timer;

namespace EspionSpotify
{
    public sealed class Watcher : IWatcher, IDisposable
    {
        private const int NEXT_SONG_EVENT_MAX_ESTIMATED_DELAY_SECS = 5;
        private const int WATCHER_DELAY_MS = 500;
        private readonly IMainAudioSession _audioSession;
        private AudioThrottler _audioThrottler;
        private readonly IFileSystem _fileSystem;

        private readonly IFrmEspionSpotify _form;
        private readonly List<RecorderTask> _recorderTasks;
        private readonly UserSettings _userSettings;
        private Track _currentTrack;
        private bool _disposed;
        private bool _isPlaying;

        private Timer _recordingTimer;
        private bool _stopRecordingWhenSongEnds;
        private readonly CancellationTokenSource _cancellationToken;

        internal Watcher(IFrmEspionSpotify form, IMainAudioSession audioSession, UserSettings userSettings) :
            this(form, audioSession, userSettings, new Track(), new FileSystem(), new List<RecorderTask>())
        {
        }

        public Watcher(IFrmEspionSpotify form, IMainAudioSession audioSession, UserSettings userSettings, Track track,
            IFileSystem fileSystem, List<RecorderTask> recorderTasks)
        {
            _form = form;
            _audioSession = audioSession;
            _userSettings = userSettings;
            _currentTrack = track;
            _fileSystem = fileSystem;
            _recorderTasks = recorderTasks;
            
            _cancellationToken = new CancellationTokenSource();
            
            Settings.Default.app_console_logs = string.Empty;
            Settings.Default.Save();
        }

        public static bool Running { get; internal set; }
        public static bool Ready { get; private set; } = true;
        public static bool ToggleStopRecordingDelayed { get; internal set; }

        public bool IsRecordUnknownActive => !_userSettings.MuteAdsEnabled && _userSettings.RecordEverythingEnabled &&
                                             (_currentTrack.IsUnknownPlaying || _userSettings.RecordAdsEnabled);

        public bool IsMaxOrderNumberAsFileExceeded => _userSettings.OrderNumberInfrontOfFileEnabled &&
                                                      _userSettings.OrderNumberAsFile == _userSettings.OrderNumberMax;

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        public int CountSeconds { get; set; }
        public ISpotifyHandler Spotify { get; set; }

        public bool RecorderUpAndRunning => _recorderTasks.Any(x => 
            !x.Task.IsCompleted && 
            x.Recorder.Track.Equals(_currentTrack) && 
            x.Recorder != null && 
            x.Recorder.Running);

        public bool IsTypeAllowed => _currentTrack.IsNormalPlaying || IsRecordUnknownActive;

        public bool IsOldSong =>
            _userSettings.EndingTrackDelayEnabled && _currentTrack.Length > 0
                                                  && _currentTrack.CurrentPosition > Math.Max(0,
                                                      (_currentTrack.Length ?? 0) -
                                                      NEXT_SONG_EVENT_MAX_ESTIMATED_DELAY_SECS);

        public async Task Run()
        {
            if (Running) return;

            _form.WriteIntoConsole(I18NKeys.LogStarting);

            NativeMethods.PreventSleep();

            await RunSpotifyConnect();
            var isAudioSessionNotFound = !await SetSpotifyAudioSessionAndWaitToStart();
            BindSpotifyEventHandlers();
            Ready = false;

            if (Recorder.TestFileWriter(_form, _audioSession, _userSettings))
            {
                if (isAudioSessionNotFound)
                {
                    _form.WriteIntoConsole(I18NKeys.LogSpotifyPlayingOutsideOfSelectedAudioEndPoint);
                    Running = false;
                }
                else if (SpotifyConnect.IsSpotifyRunning())
                {
                    await InitializeRecordingSession();

                    while (Running)
                    {
                        // Order is important
                        if (!SpotifyConnect.IsSpotifyRunning())
                        {
                            _form.WriteIntoConsole(I18NKeys.LogSpotifyIsClosed);
                            Running = false;
                        }
                        else if (ToggleStopRecordingDelayed)
                        {
                            ToggleStopRecordingDelayed = false;
                            _stopRecordingWhenSongEnds = true;
                            _form.WriteIntoConsole(I18NKeys.LogStopRecordingWhenSongEnds);
                        }
                        else if (!_stopRecordingWhenSongEnds && _userSettings.HasRecordingTimerEnabled &&
                                 !_recordingTimer.Enabled)
                        {
                            _form.WriteIntoConsole(I18NKeys.LogRecordingTimerDone);
                            ToggleStopRecordingDelayed = true;
                        }

                        await Task.Delay(WATCHER_DELAY_MS);
                    }

                    DoIKeepLastSong();
                    StopLastRecorder();
                }
                else if (SpotifyConnect.IsSpotifyInstalled(_fileSystem))
                {
                    _form.WriteIntoConsole(isAudioSessionNotFound
                        ? I18NKeys.LogSpotifyIsClosed
                        : I18NKeys.LogSpotifyNotConnected);
                }
                else
                {
                    _form.WriteIntoConsole(I18NKeys.LogSpotifyNotFound);
                }
            }

            EndRecordingSession();

            _form.WriteIntoConsole(I18NKeys.LogStoping);

            NativeMethods.AllowSleep();
        }

        private void OnPlayStateChanged(object sender, PlayStateEventArgs e)
        {
            // it will be triggered after onTrackChanged from track_Spotify playing (fading out) to track_Spotify paused

            if (e.Playing == _isPlaying) return;
            _isPlaying = e.Playing;

            // was paused
            if (!_isPlaying && _recorderTasks.Any(x => x.Task.Status == TaskStatus.Running))
            {
                _form.UpdateNumUp();
            }

            MutesSpotifyAds(_currentTrack.Ad);

            _form.UpdateIconSpotify(_isPlaying);
        }

        private void OnTrackChanged(object sender, TrackChangeEventArgs e)
        {
            // do not add "is current track an ad" validation, audio is already muted
            if (RecorderUpAndRunning && IsOldSong)
            {
               // _audioSession.SleepWhileTheSongEnds();
            }

            if (!IsNewTrack(e.NewTrack)) return;

            DoIKeepLastSong();
            StopLastRecorder();

            if (IsMaxOrderNumberAsFileExceeded)
            {
                _form.WriteIntoConsole(I18NKeys.LogMaxFileSequenceReached, _userSettings.OrderNumberMax);
            }

            if (!_isPlaying || RecorderUpAndRunning || !IsTypeAllowed || IsMaxOrderNumberAsFileExceeded) return;

            RecordSpotify();
        }

        private void OnTrackTimeChanged(object sender, TrackTimeChangeEventArgs e)
        {
            _currentTrack.CurrentPosition = e.TrackTime;
            _form.UpdateRecordedTime(RecorderUpAndRunning ? (int?) e.TrackTime : null);
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

            var isAd = !IsRecordUnknownActive && _currentTrack.Ad && !_currentTrack.ToString().IsSpotifyIdleState();
            var adTitle = isAd
                ? $"{_form.Rm?.GetString(I18NKeys.LblAd) ?? "Ad"}: "
                : "";
            _form.UpdatePlayingTitle($"{adTitle}{_currentTrack}");

            MutesSpotifyAds(isAd);

            if (isAd)
            {
                _form.WriteIntoConsole(I18NKeys.LogAd);
            }

            return true;
        }

        private async Task RunSpotifyConnect()
        {
            if (!SpotifyConnect.IsSpotifyInstalled(_fileSystem)) return;

            if (!SpotifyConnect.IsSpotifyRunning())
            {
                _form.WriteIntoConsole(I18NKeys.LogSpotifyConnecting);
                await SpotifyConnect.Run(_fileSystem);
            }

            Running = true;
        }

        private async Task<bool> SetSpotifyAudioSessionAndWaitToStart()
        {
            _audioSession.SetSpotifyProcesses();
            _audioSession.RouteSpotifyAudioSessions();
            return await _audioSession.WaitSpotifyAudioSessionToStart(Running);
        }

        private void BindSpotifyEventHandlers()
        {
            Spotify = new SpotifyHandler(_audioSession);

            Spotify.OnPlayStateChange += OnPlayStateChanged;
            Spotify.OnTrackChange += OnTrackChanged;
            Spotify.OnTrackTimeChange += OnTrackTimeChanged;
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

            var recorder = new Recorder(
                form: _form, 
                audioThrottler: _audioThrottler,
                userSettings: _userSettings,
                track: ref _currentTrack,
                fileSystem: _fileSystem);

            _recorderTasks.Add(new RecorderTask
            {
                Task = Task.Run(() => recorder.Run(_cancellationToken), _cancellationToken.Token),
                Recorder = recorder,
                Token = _cancellationToken
            });

            _form.UpdateIconSpotify(_isPlaying, true);
        }

        private async Task InitializeRecordingSession()
        {
            _audioSession.SetSpotifyVolumeToHighAndOthersToMute(true);
            _audioSession.SetSpotifyToMute(false);

            var track = await Spotify.GetTrack();

            if (track != null)
            {
                _isPlaying = track.Playing;
                _form.UpdateIconSpotify(_isPlaying);

                _form.UpdatePlayingTitle(track.ToString());
                MutesSpotifyAds(track.Ad);
            }

            Spotify.ListenForEvents = true;

            _audioThrottler = new AudioThrottler(_audioSession);
#pragma warning disable CS4014
            Task.Run(() => _audioThrottler.Run(_cancellationToken), _cancellationToken.Token);
#pragma warning restore CS4014

            if (_userSettings.HasRecordingTimerEnabled) await Task.Run(EnableRecordingTimer);
        }

        private async Task EnableRecordingTimer()
        {
            _recordingTimer = new Timer(_userSettings.RecordingTimerMilliseconds)
            {
                AutoReset = false,
                Enabled = false
            };

            while (_recorderTasks.Count == 0 && SpotifyConnect.IsSpotifyRunning()) await Task.Delay(300);

            _recordingTimer.Enabled = true;
        }

        private void ResetAudioSession()
        {
            if (_audioSession != null)
            {
                MutesSpotifyAds(false);
                _audioSession.SetSpotifyVolumeToHighAndOthersToMute(mute: false);
                _audioSession.ClearSpotifyAudioSessionControls();
                _audioSession.UnrouteSpotifyAudioSessions();
            }
            
            _audioThrottler.Running = false;
        }

        private void ResetSpotifyHandler()
        {
            if (Spotify != null)
            {
                Spotify.ListenForEvents = false;
                Spotify.OnPlayStateChange -= OnPlayStateChanged;
                Spotify.OnTrackChange -= OnTrackChanged;
                Spotify.OnTrackTimeChange -= OnTrackTimeChanged;
                Spotify.Dispose();
            }
        }

        private void EndRecordingSession()
        {
            Ready = true;

            ResetAudioSession();
            ResetSpotifyHandler();

            _form.UpdateStartButton();
            _form.UpdatePlayingTitle(Constants.SPOTIFY);
            _form.UpdateIconSpotify(false);
            _form.UpdateRecordedTime(null);
            _form.StopRecording();
        }

        private void ManageRecorderTasks()
        {
            if (_recorderTasks.Count > 0) _form.UpdateNumUp();
            _recorderTasks.RemoveAll(x => x.Task.Status == TaskStatus.RanToCompletion);
        }

        private void DoIKeepLastSong()
        {
            // always increment when session ends
            if (!Running && _recorderTasks.Any(t => t.Task.Status != TaskStatus.RanToCompletion)) _form.UpdateNumUp();
            // valid if the track is removed, go back one count
            if (RecorderUpAndRunning && CountSeconds < _userSettings.MinimumRecordedLengthSeconds)
                _form.UpdateNumDown();
        }

        private void StopLastRecorder()
        {
            var recorder = _recorderTasks.LastOrDefault()?.Recorder;
            if (recorder == null || _recorderTasks.Count == 0 || _recorderTasks.Last().Task.IsCompleted) return;
            if (recorder.Running)
            {
                recorder.Running = false;
                recorder.CountSeconds = CountSeconds;
            }
            
            _form.UpdateIconSpotify(_isPlaying);
        }

        private void MutesSpotifyAds(bool value)
        {
            if (_userSettings.MuteAdsEnabled) _audioSession.SetSpotifyToMute(value);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                DoIKeepLastSong();
                NativeMethods.AllowSleep();

                ResetAudioSession();
                ResetSpotifyHandler();

                _recorderTasks.ForEach(x =>
                {
                    x.Token.Cancel();
                    try
                    {
                        x.Task.Wait();
                    }
                    catch
                    {
                        // ignored
                    }
                    finally
                    {
                        x.Task.Dispose();
                    }
                });

                if (_audioThrottler != null)
                {
                    _audioThrottler.Dispose();
                    _audioThrottler = null;
                }
            }

            _disposed = true;
        }
    }
}