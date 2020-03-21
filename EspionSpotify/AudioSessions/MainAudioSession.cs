using Microsoft.Win32.SafeHandles;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace EspionSpotify.AudioSessions
{
    public class MainAudioSession : IMainAudioSession, IDisposable
    {
        private const int SLEEP_VALUE = 50;
        private const int NUMBER_OF_SAMPLES = 3;

        private bool _disposed = false;
        private readonly SafeHandle _disposeHandle = new SafeFileHandle(IntPtr.Zero, true);

        private readonly Process _spytifyProcess;
        private ICollection<int> _spotifyProcessesIds;

        public MMDeviceEnumerator AudioMMDevices { get; private set; }
        public AudioMMDevicesManager AudioMMDevicesManager { get; private set; }
        public int AudioDeviceVolume { get => (int)(AudioMMDevicesManager.AudioEndPointDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);  }
        public bool IsAudioEndPointDeviceIndexAvailable { get => AudioMMDevicesManager.AudioEndPointDeviceNames.ContainsKey(AudioMMDevicesManager.AudioEndPointDeviceID); }

        public ICollection<AudioSessionControl> SpotifyAudioSessionControls { get; private set; } = new List<AudioSessionControl>();

        private SessionCollection GetSessionsAudioEndPointDevice => AudioMMDevicesManager.GetAudioEndPointDeviceSessions;

        public MainAudioSession(string audioEndPointDeviceID)
        {
            _spytifyProcess = Process.GetCurrentProcess();

            AudioMMDevices = new MMDeviceEnumerator();
            AudioMMDevicesManager = new AudioMMDevicesManager(AudioMMDevices, audioEndPointDeviceID);

            AudioMMDevices.RegisterEndpointNotificationCallback(AudioMMDevicesManager);
        }

        public void SetAudioDeviceVolume(int volume)
        {
            if (AudioMMDevicesManager.volumeNotificationEmitted)
            {
                AudioMMDevicesManager.volumeNotificationEmitted = false;
                return;
            }
            if (float.TryParse(volume.ToString(), out var fNewVolume))
            {
                AudioMMDevicesManager.AudioEndPointDevice.AudioEndpointVolume.MasterVolumeLevelScalar = fNewVolume / 100;
            }
        }

        public async void SleepWhileTheSongEnds()
        {
            for (var times = 1000; await IsSpotifyCurrentlyPlaying() && times > 0; times -= SLEEP_VALUE * NUMBER_OF_SAMPLES)
            {
                await Task.Delay(SLEEP_VALUE);
            }
        }

        public async Task<bool> IsSpotifyCurrentlyPlaying()
        {
            var samples = new List<double>();

            for (var sample = 0; sample < NUMBER_OF_SAMPLES; sample++)
            {
                var spotifySoundValue = 0.0;
                await Task.Delay(SLEEP_VALUE);

                var spotifyAudioSessionControls = SpotifyAudioSessionControls;
                lock (spotifyAudioSessionControls)
                {
                    foreach (var audioSession in spotifyAudioSessionControls)
                    {
                        var soundValue = Math.Round(audioSession.AudioMeterInformation.MasterPeakValue * 100.0, 1);
                        if (soundValue == 0.0) continue;

                        spotifySoundValue = soundValue;
                    }
                }

                samples.Add(spotifySoundValue);
            }

            return samples.DefaultIfEmpty().Average() > 1.0;
        }

        public void SetSpotifyToMute(bool mute)
        {
            var spotifyAudioSessionControlsLocked = SpotifyAudioSessionControls;
            lock (spotifyAudioSessionControlsLocked)
            {
                foreach (var audioSession in spotifyAudioSessionControlsLocked)
                {
                    audioSession.SimpleAudioVolume.Mute = mute;
                }
            }
        }

        public async Task<bool> WaitSpotifyAudioSessionToStart(bool running)
        {
            _spotifyProcessesIds = SpotifyProcess.GetSpotifyProcesses().Select(x => x.Id).ToList();

            if (await IsSpotifyPlayingOutsideDefaultAudioEndPoint(running))
            {
                return false;
            }

            var sessionAudioEndPointDeviceLocked = GetSessionsAudioEndPointDevice;

            for (var i = 0; i < sessionAudioEndPointDeviceLocked.Count; i++)
            {
                var currentAudioSessionControl = sessionAudioEndPointDeviceLocked[i];
                var currentProcessId = (int)currentAudioSessionControl.GetProcessID;
                if (!IsSpotifyAudioSessionControl(currentProcessId)) continue;

                return true;
            }

            return false;
        }

        public void SetSpotifyVolumeToHighAndOthersToMute(bool mute)
        {
            var sessionAudioEndPointDeviceLocked = GetSessionsAudioEndPointDevice;

            for (var i = 0; i < sessionAudioEndPointDeviceLocked.Count; i++)
            {
                var currentAudioSessionControl = sessionAudioEndPointDeviceLocked[i];
                var currentProcessId = (int)currentAudioSessionControl.GetProcessID;

                if (currentProcessId.Equals(_spytifyProcess.Id)) continue;

                if (IsSpotifyAudioSessionControl(currentProcessId))
                {
                    SpotifyAudioSessionControls.Add(currentAudioSessionControl);

                    if (currentAudioSessionControl.SimpleAudioVolume.Volume < 1)
                    {
                        currentAudioSessionControl.SimpleAudioVolume.Volume = 1;
                    }
                }
                else if (!currentAudioSessionControl.SimpleAudioVolume.Mute.Equals(mute))
                {
                    currentAudioSessionControl.SimpleAudioVolume.Mute = mute;
                }
            }
        }

        private async Task<bool> IsSpotifyPlayingOutsideDefaultAudioEndPoint(bool running)
        {
            int? spotifyAudioSessionProcessId = null;

            while (running && spotifyAudioSessionProcessId == null && _spotifyProcessesIds.Any())
            {
                var allSessionsAudioEndPointDevices = AudioMMDevices.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active).Select(x => x.AudioSessionManager.Sessions).ToArray();

                lock (allSessionsAudioEndPointDevices)
                {
                    foreach (var sessionAudioEndPointDevice in allSessionsAudioEndPointDevices)
                    {
                        for (var i = 0; i < sessionAudioEndPointDevice.Count; i++)
                        {
                            var currentAudioSessionControl = sessionAudioEndPointDevice[i];
                            var currentProcessId = (int)currentAudioSessionControl.GetProcessID;
                            if (!IsSpotifyAudioSessionControl(currentProcessId)) continue;

                            spotifyAudioSessionProcessId = currentProcessId;
                            break;
                        }
                        if (spotifyAudioSessionProcessId.HasValue) break;
                    }
                }

                await Task.Delay(300);

                _spotifyProcessesIds = SpotifyProcess.GetSpotifyProcesses().Select(x => x.Id).ToList();
            }

            var sessionAudioSelectedEndPointDevice = GetSessionsAudioEndPointDevice;

            for (var i = 0; i < sessionAudioSelectedEndPointDevice.Count; i++)
            {
                var currentAudioSessionControl = sessionAudioSelectedEndPointDevice[i];
                var currentProcessId = (int)currentAudioSessionControl.GetProcessID;
                if (currentProcessId != spotifyAudioSessionProcessId) continue;

                return false;
            }

            return true;
        }

        private bool IsSpotifyAudioSessionControl(int processId) => _spotifyProcessesIds.Any(x => x == processId);

        public void Dispose()
        {
            Dispose(true);
            AudioMMDevices.UnregisterEndpointNotificationCallback(AudioMMDevicesManager);
            AudioMMDevices.Dispose();
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _disposeHandle.Dispose();
            }

            _disposed = true;
        }
    }
}
