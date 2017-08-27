using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using NAudio.CoreAudioApi;
using static System.String;

namespace EspionSpotify
{
    internal class VolumeWin
    {
        public MMDevice DefaultAudioEndPointDevice { get; set; }
        public SessionCollection SessionsDefaultAudioEndPointDevice { get; set; }
        public MMDeviceCollection AudioEndPointDevices { get; set; }

        public VolumeWin()
        {
            var aMmDevices = new MMDeviceEnumerator();
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

        public void SetToHigh(bool bUnmute, string title)
        {
            var processes = Process.GetProcesses();
            const string spying = "Spotify";

            foreach (var process in processes)
            {
                for (var i = 0; i < SessionsDefaultAudioEndPointDevice.Count; i++)
                {
                    if (process.ProcessName.Equals(spying) 
                        && !IsNullOrEmpty(process.MainWindowTitle) 
                        && !process.ProcessName.Equals(Process.GetCurrentProcess().ProcessName)
                        && process.Id.Equals((int)SessionsDefaultAudioEndPointDevice[i].GetProcessID))
                    {
                        if (SessionsDefaultAudioEndPointDevice[i].SimpleAudioVolume.Volume < 1) SessionsDefaultAudioEndPointDevice[i].SimpleAudioVolume.Volume = 1;
                    }
                    else if (!(process.ProcessName.Equals(spying)
                               && !IsNullOrEmpty(process.MainWindowTitle)
                               || process.ProcessName.Equals(Process.GetCurrentProcess().ProcessName))
                               && process.Id.Equals((int)SessionsDefaultAudioEndPointDevice[i].GetProcessID))
                    {
                        SessionsDefaultAudioEndPointDevice[i].SimpleAudioVolume.Mute = bUnmute == false;
                    }
                }
            }
        }
    }
}
