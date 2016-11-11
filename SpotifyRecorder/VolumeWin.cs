using System;
using System.Collections.Generic;
using System.Diagnostics;
using NAudio.CoreAudioApi;

namespace EspionSpotify
{
    class VolumeWin
    {
        private MMDeviceEnumerator aMMDevices;


        public MMDevice DefaultAudioEndPointDevice { get; set; }
        public SessionCollection SessionsDefaultAudioEndPointDevice { get; set; }

        public MMDeviceCollection AudioEndPointDevices { get; set; }

        public VolumeWin()
        {
            aMMDevices = new MMDeviceEnumerator();
            DefaultAudioEndPointDevice = aMMDevices.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            SessionsDefaultAudioEndPointDevice = DefaultAudioEndPointDevice.AudioSessionManager.Sessions;
            DefaultAudioDeviceVolume = (int)(DefaultAudioEndPointDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
        }

        public int DefaultAudioDeviceVolume { get; set; }
        public void SetDefaultAudioDeviceVolume(int Volume)
        {
            float fNewVolume = (float)0.0;
            if (float.TryParse(Volume.ToString(), out fNewVolume))
                DefaultAudioEndPointDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (fNewVolume / 100);
        }

        public void SetToHigh(bool bUnmute, string title, string Spying)
        {
            Process[] Processes = Process.GetProcesses();
            List<Process> processToMute = new List<Process>();
            foreach (Process process in Processes)
            {
                for (int i = 0; i < SessionsDefaultAudioEndPointDevice.Count; i++)
                {
                    if (Spying == "Spotify" || Spying == "chrome")
                    {
                        if ((process.ProcessName.Equals(Spying)
                            && !String.IsNullOrEmpty(process.MainWindowTitle)
                            || (process.ProcessName.Equals(Process.GetCurrentProcess().ProcessName))) &&
                                ((process.Id.Equals((int)(SessionsDefaultAudioEndPointDevice[i].GetProcessID)))))
                        {
                            if (SessionsDefaultAudioEndPointDevice[i].SimpleAudioVolume.Volume < 1)
                                SessionsDefaultAudioEndPointDevice[i].SimpleAudioVolume.Volume = 1;
                        }
                        else if (!(process.ProcessName.Equals(Spying)
                            && !String.IsNullOrEmpty(process.MainWindowTitle)
                        || (process.ProcessName.Equals(Process.GetCurrentProcess().ProcessName))) &&
                            ((process.Id.Equals((int)(SessionsDefaultAudioEndPointDevice[i].GetProcessID)))))
                        {
                            if (bUnmute == false)
                            {
                                SessionsDefaultAudioEndPointDevice[i].SimpleAudioVolume.Mute = true;
                            }
                            else if (bUnmute == true)
                            {
                                SessionsDefaultAudioEndPointDevice[i].SimpleAudioVolume.Mute = false;
                            }
                        }
                    }                    
                }
            }
        }
    }
}
