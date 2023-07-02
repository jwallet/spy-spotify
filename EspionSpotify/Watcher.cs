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
using EspionSpotify.Router;
using EspionSpotify.Spotify;
using EspionSpotify.Translations;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using Timer = System.Timers.Timer;

namespace EspionSpotify
{
    public sealed class Watcher : IWatcher, IDisposable
    {
        private const int WATCHER_DELAY_MS = 500;
        private readonly IMainAudioSession _audioSession;
        private IAudioThrottler _audioThrottler;
        private readonly IFileSystem _fileSystem;
        private readonly IProcessManager _processManager;
        private IAudioLoopback _audioLoopback;
        private IAudioRouter _audioRouter;

        private readonly IFrmEspionSpotify _form;
        private readonly List<RecorderTask> _recorderTasks;
        private readonly UserSettings _userSettings;
        private Track _currentTrack;
        private bool _disposed;
        private bool _isPlaying;

        private Timer _recordingTimer;
        private bool _stopRecordingWhenSongEnds;
        private readonly CancellationTokenSource _cancellationTokenSource;

        internal Watcher(IFrmEspionSpotify form, IMainAudioSession audioSession, UserSettings userSettings) : this(
            form,
            audioSession,
            userSettings,
            new AudioThrottler(audioSession),
            new AudioRouter(DataFlow.Render),
            new AudioLoopback(
                currentEndpointDevice: audioSession.AudioMMDevicesManager.AudioEndPointDevice,
                defaultEndpointDeviceIdentifier: audioSession.AudioMMDevicesManager.AudioMMDevices.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia).ID),
            new Track(),
            new FileSystem(),
            new ProcessManager(),
            new List<RecorderTask>())
        { }

        public Watcher(IFrmEspionSpotify form, IMainAudioSession audioSession, UserSettings userSettings, IAudioThrottler audioThrottler, IAudioRouter audioRouter, IAudioLoopback audioLoopback, Track track,
            IFileSystem fileSystem, IProcessManager processManager, List<RecorderTask> recorderTasks)
        {
            _form = form;
            _audioSession = audioSession;
            _userSettings = userSettings;
            _audioThrottler = audioThrottler;
            _currentTrack = track;
            _fileSystem = fileSystem;
            _processManager = processManager;
            _recorderTasks = recorderTasks;
            _audioRouter = audioRouter;
            _audioLoopback = audioLoopback;
            
            _cancellationTokenSource = new CancellationTokenSource();
            
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

        public ISpotifyHandler Spotify { get; set; }

        public bool RecorderUpAndRunning => _recorderTasks.Any(x => 
            !x.Task.IsCompleted && 
            x.Recorder != null && 
            x.Recorder.Running &&
            x.Recorder.Track.Equals(_currentTrack)
        );

        public bool IsTypeAllowed => _currentTrack.IsNormalPlaying || IsRecordUnknownActive;

        public async Task Run()
        {
            if (Running) return;

            _form.WriteIntoConsole(I18NKeys.LogStarting);

            NativeMethods.PreventSleep();

            Ready = false;

            if (Recorder.TestFileWriter(_form, _audioSession, _userSettings))
            {
                await RunSpotifyConnect();
                var isAudioSessionNotFound = !await SetSpotifyAudioSessionAndWaitToStart();
                BindSpotifyEventHandlers();

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
                    _form.UpdateIconSpotify(_isPlaying);
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
            if (!IsNewTrack(e.NewTrack, e.OldTrack)) return;

            StopLastRecorder();
            UpdateTrackPositionOfLastRecorder(e.OldTrack.CurrentPosition);

            _currentTrack = e.NewTrack;
            _isPlaying = _currentTrack.Playing;

            var canRecord = _isPlaying && !RecorderUpAndRunning && IsTypeAllowed && !IsMaxOrderNumberAsFileExceeded;
            if (canRecord)
            {
                RecordSpotify();
            }
            
            DoIKeepLastSong();

            if (!canRecord)
            {
                _form.UpdateIconSpotify(_isPlaying);
            }
            
            if (IsMaxOrderNumberAsFileExceeded)
            {
                _form.WriteIntoConsole(I18NKeys.LogMaxFileSequenceReached, _userSettings.OrderNumberMax);
            }

            UpdateInterfaceOnNewTrack();
        }

        private void OnTrackTimeChanged(object sender, TrackTimeChangeEventArgs e)
        {
            _currentTrack.CurrentPosition = e.TrackTime;
            _form.UpdateRecordedTime(RecorderUpAndRunning ? (int?) e.TrackTime : null);
        }

        private void UpdateInterfaceOnNewTrack()
        {
            var isAd = !IsRecordUnknownActive && _currentTrack.Ad && !_currentTrack.ToString().IsSpotifyIdleState();
            var adTitle = isAd
                ? $"{_form.Rm?.GetString(I18NKeys.LblAd) ?? "Ad"}: "
                : "";
            _form.UpdatePlayingTitle($"{adTitle}{_currentTrack}");
            _form.UpdateRecordedTime(RecorderUpAndRunning ? 0 : null);

            // will mute even if the window title is "Spotify"
            MutesSpotifyAds(isAd || _currentTrack.ToString().IsNullOrAdOrSpotifyIdleState());

            if (isAd)
            {
                _form.WriteIntoConsole(I18NKeys.LogAd);
            }
        }

        public bool IsNewTrack(Track newTrack, Track oldTrack)
        {
            if (newTrack == null || new Track().Equals(newTrack)) return false;

            if (oldTrack is Track && oldTrack.Equals(newTrack))
            {
                _form.UpdateIconSpotify(_isPlaying, RecorderUpAndRunning);
                return false;
            }

            return true;
        }

        private async Task RunSpotifyConnect()
        {
            if (SpotifyConnect.IsSpotifyInstalled(_fileSystem))
            {
                if (!SpotifyConnect.IsSpotifyRunning())
                {
                    _form.WriteIntoConsole(I18NKeys.LogSpotifyConnecting);
                    await SpotifyConnect.Run(_fileSystem);
                }
            };

            Running = true;
        }

        private async Task<bool> SetSpotifyAudioSessionAndWaitToStart()
        {
            _audioSession.SetSpotifyProcesses();
            RouteSpotifyAudioSessions(canRedirectPlayback: _userSettings.ListenToSpotifyPlaybackEnabled);
            await SpotifyConnect.StartAudioSession(_processManager);
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

            var recorder = new Recorder(
                form: _form, 
                audioThrottler: _audioThrottler,
                userSettings: _userSettings,
                track: ref _currentTrack,
                fileSystem: _fileSystem);

            _recorderTasks.Add(new RecorderTask
            {
                Task = Task.Run(() => recorder.Run(_cancellationTokenSource), _cancellationTokenSource.Token),
                Recorder = recorder,
                Token = _cancellationTokenSource
            });
            
            _form.UpdateIconSpotify(_isPlaying, true);
        }

        private async Task InitializeRecordingSession()
        {
            Spotify.ListenForEvents = true;

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

            var t = new Task(() => _audioThrottler.Run(_cancellationTokenSource), _cancellationTokenSource.Token);
            t.RunSynchronously();

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
            MutesSpotifyAds(false);
            _audioSession.SetSpotifyVolumeToHighAndOthersToMute(mute: false);
            _audioSession.ClearSpotifyAudioSessionControls();

            if (_audioThrottler.Running)
            {
                _audioThrottler.Running = false;
            }

            if (_audioLoopback.Running)
            {
                _audioLoopback.Running = false;
            }

            _audioRouter.ResetDefaultEndpoints();
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
            _recorderTasks.RemoveAll(x => new[]
            {
                TaskStatus.Canceled,
                TaskStatus.RanToCompletion,
                TaskStatus.Faulted,
            }.Contains(x.Task.Status));
        }

        private void DoIKeepLastSong()
        {
            // always increment when session ends
            if (!Running && _recorderTasks.Any(t => t.Task.Status != TaskStatus.RanToCompletion)) _form.UpdateNumUp();
            // valid if the track is removed, go back one count
            if (RecorderUpAndRunning && _currentTrack.CurrentPosition < _userSettings.MinimumRecordedLengthSeconds)
                _form.UpdateNumDown();
        }

        private void UpdateTrackPositionOfLastRecorder(int? trackPosition)
        {
            if (_recorderTasks.Count == 0 || !trackPosition.HasValue) return;
            var recorderTask = _recorderTasks.LastOrDefault();
            if (recorderTask == null || recorderTask.Task.IsCompleted) return;
            recorderTask.Recorder.Track.CurrentPosition = trackPosition;
        }

        private void StopLastRecorder()
        {
            if (_recorderTasks.Count == 0) return;
            var recorderTask = _recorderTasks.LastOrDefault();
            if (recorderTask == null || recorderTask.Task.IsCompleted) return;
            if (recorderTask.Recorder.Running)
            {
                recorderTask.Recorder.Stop();
            }
        }

        private void MutesSpotifyAds(bool value)
        {
            // run all apps to prevent video ads to run on a different audio session.
            if (_userSettings.MuteAdsEnabled) _audioSession.SetSpotifyVolumeToHighAndOthersToMute(value);
        }

        private void RouteSpotifyAudioSessions(bool canRedirectPlayback)
        {
            foreach (var spotifyProcessesId in _audioSession.SpotifyProcessesIds)
            {
                _audioRouter.SetDefaultEndPoint(
                    _audioSession.AudioMMDevicesManager.AudioEndPointDeviceID,
                    spotifyProcessesId);
            }

            if (canRedirectPlayback)
            {
                Task.Run(() => _audioLoopback.Run(_cancellationTokenSource));
            }
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

                if (_audioRouter != null)
                {
                    _audioRouter.ResetDefaultEndpoints();
                    _audioRouter = null;
                }

                if (_audioLoopback != null)
                {
                    _audioLoopback.Dispose();
                    _audioLoopback = null;
                }

                if (_audioThrottler != null)
                {
                    _audioThrottler.Running = false;
                    _audioThrottler.Dispose();
                    _audioThrottler = null;
                }
            }

            _disposed = true;
        }
    }
}