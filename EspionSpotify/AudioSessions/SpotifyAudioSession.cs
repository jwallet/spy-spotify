using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EspionSpotify.Extensions;

namespace EspionSpotify.AudioSessions
{
    public class SpotifyAudioSession: MainAudioSession, ISpotifyAudioSession
    {
        private const int _sleepValue = 50;
        private const int _numberOfSamples = 3;

        private Process _spytifyProcess;
        private readonly ICollection<int> _spotifyProcessesIds;
        private ICollection<AudioSessionControl> _spotifyAudioSessionControls;

        private SessionCollection GetSessionsDefaultAudioEndPointDevice => DefaultAudioEndPointDevice.AudioSessionManager.Sessions;

        public SpotifyAudioSession()
        {
            _spotifyProcessesIds = SpotifyProcess.GetSpotifyProcesses().Select(x => x.Id).ToList();
            _spytifyProcess = Process.GetCurrentProcess();
            _spotifyAudioSessionControls = new List<AudioSessionControl>();
        }

        public void SleepWhileTheSongEnds()
        {
            for (var times = 1000; IsSpotifyCurrentlyPlaying() && times > 0; times -= _sleepValue * _numberOfSamples)
            {
                Thread.Sleep(_sleepValue);
            }
        }

        public bool IsSpotifyCurrentlyPlaying()
        {
            var samples = new List<double>();

            for (var sample = 0; sample < _numberOfSamples; sample++)
            {
                var spotifySoundValue = 0.0;
                Thread.Sleep(_sleepValue);

                lock (_spotifyAudioSessionControls)
                {
                    foreach (var audioSession in _spotifyAudioSessionControls)
                    {
                        var soundValue = Math.Round(audioSession.AudioMeterInformation.MasterPeakValue * 100.0, 1);
                        if (soundValue == 0.0) continue;

                        spotifySoundValue = soundValue;
                    }
                }

                samples.Add(spotifySoundValue);
            }

            return samples.DefaultIfEmpty().Average() > 0.0;
        }

        public void SetSpotifyToMute(bool mute)
        {
            lock (_spotifyAudioSessionControls)
            {
                foreach (var audioSession in _spotifyAudioSessionControls)
                {
                    audioSession.SimpleAudioVolume.Mute = mute;
                }
            }
        }

        public async Task WaitSpotifyAudioSessionToStart()
        {
            var spotifyAudioSessionStarted = false;

            while (!spotifyAudioSessionStarted)
            {
                var sessionDefaultAudioEndPointDevice = GetSessionsDefaultAudioEndPointDevice;

                for (var i = 0; i < sessionDefaultAudioEndPointDevice.Count; i++)
                {
                    var currentAudioSessionControl = sessionDefaultAudioEndPointDevice[i];
                    var currentProcessId = (int)currentAudioSessionControl.GetProcessID;
                    if (!IsSpotifyAudioSessionControl(currentProcessId)) continue;

                    spotifyAudioSessionStarted = true;
                }

                UpdateDefaultAudioEndPointDevice(new MMDeviceEnumerator());
                await Task.Delay(100);
            }
        }

        public void SetSpotifyVolumeToHighAndOthersToMute(bool mute)
        {
            var sessionDefaultAudioEndPointDevice = GetSessionsDefaultAudioEndPointDevice;

            for (var i = 0; i < sessionDefaultAudioEndPointDevice.Count; i++)
            {
                var currentAudioSessionControl = sessionDefaultAudioEndPointDevice[i];
                var currentProcessId = (int)currentAudioSessionControl.GetProcessID;

                if (currentProcessId.Equals(_spytifyProcess.Id)) continue;

                if (IsSpotifyAudioSessionControl(currentProcessId))
                {
                    _spotifyAudioSessionControls.Add(currentAudioSessionControl);

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

        private bool IsSpotifyAudioSessionControl(int processId) => _spotifyProcessesIds.Any(x => x == processId);
    }
}
