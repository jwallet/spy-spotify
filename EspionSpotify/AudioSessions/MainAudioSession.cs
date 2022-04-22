using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EspionSpotify.Extensions;
using EspionSpotify.Native;
using EspionSpotify.Router;
using EspionSpotify.Spotify;
using NAudio.CoreAudioApi;

namespace EspionSpotify.AudioSessions
{
    public sealed class MainAudioSession : IMainAudioSession, IDisposable
    {
        private const int SLEEP_VALUE = 50;
        private const int NUMBER_OF_SAMPLES = 3;

        private readonly IProcessManager _processManager;
        private readonly IAudioRouter _audioRouter;
        private readonly AudioLoopback _audioLoopback;

        private readonly int? _spytifyProcessId;
        private int? _spotifyAudioSessionProcessId;
        
        private bool _disposed;
        private CancellationTokenSource _cancellationTokenSource;
        private ICollection<int> _spotifyProcessesIds = new List<int>();
        
        internal MainAudioSession(string audioEndPointDeviceID) :
            this(audioEndPointDeviceID, new ProcessManager(), new AudioRouter(DataFlow.Render))
        {
        }

        private MainAudioSession(string audioEndPointDeviceID, IProcessManager processManager, IAudioRouter audioRouter)
        {
            _processManager = processManager;
            _spytifyProcessId = _processManager.GetCurrentProcess()?.Id;
            
            _audioRouter = audioRouter;
            
            AudioMMDevices = new MMDeviceEnumerator();
            AudioMMDevicesManager = new AudioMMDevicesManager(AudioMMDevices, audioEndPointDeviceID);

            _audioLoopback = new AudioLoopback(AudioMMDevicesManager.AudioEndPointDevice,
                AudioMMDevicesManager.AudioMMDevices.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia));

            AudioMMDevices.RegisterEndpointNotificationCallback(AudioMMDevicesManager);
        }

        private SessionCollection GetSessionsAudioEndPointDevice =>
            AudioMMDevicesManager.GetAudioEndPointDeviceSessions;

        public MMDeviceEnumerator AudioMMDevices { get; private set; }
        public AudioMMDevicesManager AudioMMDevicesManager { get; }

        public int AudioDeviceVolume =>
            (int) ((AudioMMDevicesManager.AudioEndPointDevice?.AudioEndpointVolume?.MasterVolumeLevelScalar ?? 0f) *
                   100);

        public bool IsAudioEndPointDeviceIndexAvailable =>
            AudioMMDevicesManager.AudioEndPointDeviceNames.IncludesKey(AudioMMDevicesManager.AudioEndPointDeviceID);

        public ICollection<AudioSessionControl> SpotifyAudioSessionControls { get; private set; } =
            new List<AudioSessionControl>();

        public void ClearSpotifyAudioSessionControls()
        {
            SpotifyAudioSessionControls = new List<AudioSessionControl>();
        }

        public void SetSpotifyProcesses()
        {
            _spotifyProcessesIds = SpotifyProcess.GetSpotifyProcesses(_processManager).Select(x => x.Id).ToList();
        }

        public void RouteSpotifyAudioSessions()
        {
            foreach (var spotifyProcessesId in _spotifyProcessesIds)
            {
                _audioRouter.SetDefaultEndPoint(
                    AudioMMDevicesManager.AudioEndPointDeviceID,
                    spotifyProcessesId);
            }

            Task.Run(() => _audioLoopback.Run(_cancellationTokenSource));
        }

        public void UnrouteSpotifyAudioSessions()
        {
            _audioRouter.ResetDefaultEndpoints();
            _audioLoopback.Running = false;
        }

        public void SetAudioDeviceVolume(int volume)
        {
            if (AudioMMDevicesManager.AudioEndPointDevice == null) return;
            if (AudioMMDevicesManager.VolumeNotificationEmitted)
            {
                AudioMMDevicesManager.VolumeNotificationEmitted = false;
                return;
            }

            if (float.TryParse(volume.ToString(), out var fNewVolume))
                AudioMMDevicesManager.AudioEndPointDevice.AudioEndpointVolume.MasterVolumeLevelScalar =
                    fNewVolume / 100;
        }

        public async Task SleepWhileTheSongEnds()
        {
            for (var times = 1000;
                 await IsSpotifyCurrentlyPlaying() && times > 0;
                 times -= SLEEP_VALUE * NUMBER_OF_SAMPLES) await Task.Delay(SLEEP_VALUE);
        }

        #region AudioSession Spotify Playing

        public async Task<bool> IsSpotifyCurrentlyPlaying()
        {
            var samples = new List<double>();

            for (var sample = 0; sample < NUMBER_OF_SAMPLES; sample++)
            {
                var spotifySoundValue = 0.0;
                await Task.Delay(SLEEP_VALUE);

                var spotifyAudioSessionControls =
                    new List<AudioSessionControl>(SpotifyAudioSessionControls).AsReadOnly();
                foreach (var audioSession in spotifyAudioSessionControls)
                {
                    var soundValue = Math.Round(audioSession.AudioMeterInformation.MasterPeakValue * 100.0, 1);
                    if (soundValue == 0.0) continue;

                    spotifySoundValue = soundValue;
                }

                samples.Add(spotifySoundValue);
            }

            return samples.DefaultIfEmpty().Average() > 1.0;
        }

        #endregion AudioSession Spotify Playing

        #region AudioSession Spotify Muter

        public void SetSpotifyToMute(bool mute)
        {
            var spotifyAudioSessionControlsLocked =
                new List<AudioSessionControl>(SpotifyAudioSessionControls).AsReadOnly();
            foreach (var audioSession in spotifyAudioSessionControlsLocked) audioSession.SimpleAudioVolume.Mute = mute;
        }

        #endregion AudioSession Spotify Muter

        #region AudioSession Wait Spotify

        public async Task<bool> WaitSpotifyAudioSessionToStart(bool running)
        {
            if (_spotifyProcessesIds.Count == 0) return false;

            return await IsSpotifyPlayingOutsideDefaultAudioEndPoint(running);
        }

        #endregion AudioSession Wait Spotify

        #region AudioSession App Muter

        public void SetSpotifyVolumeToHighAndOthersToMute(bool mute)
        {
            var sessionAudioEndPointDeviceLocked = GetSessionsAudioEndPointDevice;

            lock (sessionAudioEndPointDeviceLocked)
            {
                for (var i = 0; i < sessionAudioEndPointDeviceLocked.Count; i++)
                {
                    var currentAudioSessionControl = sessionAudioEndPointDeviceLocked[i];
                    var currentProcessId = (int) currentAudioSessionControl.GetProcessID;

                    if (currentProcessId.Equals(_spytifyProcessId))
                    {
                        if (Math.Abs(currentAudioSessionControl.SimpleAudioVolume.Volume - 1) == 0) continue;
                        currentAudioSessionControl.SimpleAudioVolume.Volume = 1;
                    }
                    else if (IsSpotifyAudioSessionControl(currentProcessId))
                    {
                        if (currentAudioSessionControl.SimpleAudioVolume.Volume < 1)
                            currentAudioSessionControl.SimpleAudioVolume.Volume = 1;
                    }
                    else if (!currentAudioSessionControl.SimpleAudioVolume.Mute.Equals(mute))
                    {
                        currentAudioSessionControl.SimpleAudioVolume.Mute = mute;
                    }
                }
            }
        }

        #endregion AudioSession App Muter

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        #region AudioSession Spotify outside of endpoint

        private async Task<bool> IsSpotifyPlayingOutsideDefaultAudioEndPoint(bool running)
        {
            await SetSpotifyAudioSessionsAndProcessId(running);
            
            var sessionAudioEndPointDeviceLocked = GetSessionsAudioEndPointDevice;
            lock (sessionAudioEndPointDeviceLocked)
            {
                for (var i = 0; i < sessionAudioEndPointDeviceLocked.Count; i++)
                {
                    var currentAudioSessionControl = sessionAudioEndPointDeviceLocked[i];
                    var currentProcessId = (int) currentAudioSessionControl.GetProcessID;
                    if (!IsSpotifyAudioSessionControl(currentProcessId)) continue;

                    return true;
                }
            }

            return false;
        }
        
        private async Task SetSpotifyAudioSessionsAndProcessId(bool running)
        {
            _spotifyAudioSessionProcessId = null;

            while (running && _spotifyAudioSessionProcessId == null && _spotifyProcessesIds.Any())
            {
                var sessionsAudioEndPointDevices = AudioMMDevices
                    .EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)
                    .Select(x => x.AudioSessionManager.Sessions)
                    .ToList();

                lock (sessionsAudioEndPointDevices)
                {
                    foreach (var sessionAudioEndPointDevice in sessionsAudioEndPointDevices)
                    {
                        for (var i = 0; i < sessionAudioEndPointDevice.Count; i++)
                        {
                            var currentAudioSessionControl = sessionAudioEndPointDevice[i];
                            var currentProcessId = (int) currentAudioSessionControl.GetProcessID;
                            if (!IsSpotifyAudioSessionControl(currentProcessId)) continue;
                            if (!SpotifyAudioSessionControls.Contains(currentAudioSessionControl))
                            {
                                SpotifyAudioSessionControls.Add(currentAudioSessionControl);
                            }
                            _spotifyAudioSessionProcessId = currentProcessId;
                            break;
                        }

                        if (_spotifyAudioSessionProcessId.HasValue) break;
                    }
                }

                await Task.Delay(300);

                _spotifyProcessesIds = SpotifyProcess.GetSpotifyProcesses(_processManager).Select(x => x.Id).ToList();
            }
        }

        #endregion AudioSession Spotify outside of endpoint

        private bool IsSpotifyAudioSessionControl(int processId)
        {
            return _spotifyProcessesIds.Any(x => x == processId);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                AudioMMDevices.UnregisterEndpointNotificationCallback(AudioMMDevicesManager);
                AudioMMDevices.Dispose();
                AudioMMDevices = null;
                _audioLoopback.Dispose();
                _cancellationTokenSource.Cancel();
            }

            _disposed = true;
        }
    }
}