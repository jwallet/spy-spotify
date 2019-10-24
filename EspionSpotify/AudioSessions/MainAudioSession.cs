using Microsoft.Win32.SafeHandles;
using NAudio.CoreAudioApi;
using System;
using System.Runtime.InteropServices;

namespace EspionSpotify.AudioSessions
{
    public class MainAudioSession : IMainAudioSession, IDisposable
    {
        private bool _disposed = false;
        private readonly SafeHandle _disposeHandle = new SafeFileHandle(IntPtr.Zero, true);

        public MMDeviceEnumerator AudioMMDevices { get; set; }
        public int? AudioEndPointDeviceIndex { get; set; }
        public MMDevice AudioEndPointDevice { get; set; }
        public MMDevice DefaultEndPointDevice { get; set; }
        public MMDeviceCollection AudioEndPointDevices { get; set; }
        public int AudioDeviceVolume { get; set; }

        public MainAudioSession() { }

        public MainAudioSession(int? audioEndPointDeviceIndex)
        {
            AudioEndPointDeviceIndex = audioEndPointDeviceIndex;

            AudioMMDevices = new MMDeviceEnumerator();
            UpdateAudioEndPointDevices();
        }

        public void UpdateAudioEndPointDevices()
        {
            DefaultEndPointDevice = AudioMMDevices.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            AudioEndPointDevices = AudioMMDevices.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

            AudioEndPointDevice = IsAudioEndPointDeviceIndexAvailable() ? AudioEndPointDevices[AudioEndPointDeviceIndex.Value] : DefaultEndPointDevice;

            AudioDeviceVolume = (int)(AudioEndPointDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
        }

        public bool IsAudioEndPointDeviceIndexAvailable()
        {
            return AudioEndPointDeviceIndex.HasValue ? AudioEndPointDevices.Count - 1 >= AudioEndPointDeviceIndex.Value : false;
        }

        public void SetDefaultAudioDeviceVolume(int volume)
        {
            if (float.TryParse(volume.ToString(), out var fNewVolume))
            {
                AudioEndPointDevice.AudioEndpointVolume.MasterVolumeLevelScalar = fNewVolume / 100;
            }
        }

        public void Dispose()
        {
            Dispose(true);
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
