using System;
using System.Diagnostics;
using System.Threading;
using NAudio.CoreAudioApi;

namespace EspionSpotify
{
    internal class VolumeWin
    {
        public MMDevice DefaultAudioEndPointDevice { get; set; }
        public SessionCollection SessionsDefaultAudioEndPointDevice { get; set; }
        public MMDeviceCollection AudioEndPointDevices { get; set; }

        private int _spotifyVolumeSessionId;
        private const int SleepTrackChanged = 10;
        public VolumeWin()
        {
            var aMmDevices = new MMDeviceEnumerator();
            _spotifyVolumeSessionId = -1;
            DefaultAudioEndPointDevice = aMmDevices.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            SessionsDefaultAudioEndPointDevice = DefaultAudioEndPointDevice.AudioSessionManager.Sessions;
            DefaultAudioDeviceVolume = (int)(DefaultAudioEndPointDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
        }

        public int DefaultAudioDeviceVolume { get; set; }
        public void SetDefaultAudioDeviceVolume(int volume)
        {
            float fNewVolume;

            if (float.TryParse(volume.ToString(), out fNewVolume))
                DefaultAudioEndPointDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (fNewVolume / 100);
        }

        private bool IsSpotifyStillPlayingLastSong()
        {
            if (_spotifyVolumeSessionId == -1 || SessionsDefaultAudioEndPointDevice[_spotifyVolumeSessionId] == null) return false;

            return (int)Math.Round(SessionsDefaultAudioEndPointDevice[_spotifyVolumeSessionId].AudioMeterInformation.MasterPeakValue * 100) > 0;
        }

        public void SleepWhileTheSongEnds()
        {
            var times = 1000;
            while (IsSpotifyStillPlayingLastSong() && times > 0)
            {
                Thread.Sleep(SleepTrackChanged);
                times -= SleepTrackChanged;
            }
        }

        public bool SetSpotifyToMute(bool mute)
        {
            if (_spotifyVolumeSessionId == -1 || SessionsDefaultAudioEndPointDevice[_spotifyVolumeSessionId] == null) return false;

            return SessionsDefaultAudioEndPointDevice[_spotifyVolumeSessionId].SimpleAudioVolume.Mute = mute;
        }

        public void SetToHigh(bool mute = false)
        {
            var processes = Process.GetProcesses();
            const string spotify = "spotify";
            var spytify = Process.GetCurrentProcess().ProcessName;

            foreach (var process in processes)
            {
                for (var i = 0; i < SessionsDefaultAudioEndPointDevice.Count; i++)
                {
                    if (process.ProcessName.Equals(spytify) || !process.Id.Equals((int)SessionsDefaultAudioEndPointDevice[i].GetProcessID)) continue;

                    if (process.ProcessName.ToLower().Equals(spotify))
                    {
                        _spotifyVolumeSessionId = i;
                        if (SessionsDefaultAudioEndPointDevice[i].SimpleAudioVolume.Volume < 1) SessionsDefaultAudioEndPointDevice[i].SimpleAudioVolume.Volume = 1;
                    }
                    else
                    {
                        SessionsDefaultAudioEndPointDevice[i].SimpleAudioVolume.Mute = mute;
                    }
                }
            }
        }
    }
}
