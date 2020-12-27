using Microsoft.Win32.SafeHandles;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EspionSpotify.AudioSessions
{
    public class AudioMMDevicesManager : NAudio.CoreAudioApi.Interfaces.IMMNotificationClient, IDisposable
    {
        private bool _disposed = false;
        private MMDevice _defaultEndpointVolumeController;

        internal bool volumeNotificationEmitted = false;

        public MMDeviceEnumerator AudioMMDevices { get; private set; }
        public string AudioEndPointDeviceID { get; private set; }
        public string DefaultAudioEndPointDeviceID { get; private set; }
        
        public MMDevice AudioEndPointDevice {
            get
            {
                return AudioMMDevices.GetDevice(AudioEndPointDeviceNames.ContainsKey(AudioEndPointDeviceID)
                    ? AudioEndPointDeviceID
                    : DefaultAudioEndPointDeviceID);
            }
        }

        public IDictionary<string, string> AudioEndPointDeviceNames { get; private set; }

        public SessionCollection GetAudioEndPointDeviceSessions { get => AudioEndPointDevice.AudioSessionManager.Sessions; }

        public AudioMMDevicesManager(MMDeviceEnumerator audioMMDevices, string audioEndpointDeviceID)
        {
            AudioMMDevices = audioMMDevices;
            AudioEndPointDeviceID = audioEndpointDeviceID;

            AudioEndPointDeviceNames = AudioMMDevices
                .EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)
                .Select(x => new KeyValuePair<string, string>(x.ID, x.FriendlyName))
                .ToDictionary(o => o.Key, o => o.Value);

            _defaultEndpointVolumeController = AudioMMDevices.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            DefaultAudioEndPointDeviceID = _defaultEndpointVolumeController.ID;
            _defaultEndpointVolumeController.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
        }

        public void RefreshSelectedDevice(string audioEndpointDeviceID)
        {
            AudioEndPointDeviceID = audioEndpointDeviceID;
        }

        private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            if (AudioEndPointDeviceID != DefaultAudioEndPointDeviceID) return;
            var volume = (int)(data.MasterVolume * 100);
            FrmEspionSpotify.Instance.SetSoundVolume(volume);
            volumeNotificationEmitted = true;
        }

        public void OnDefaultDeviceChanged(DataFlow dataFlow, Role deviceRole, string defaultDeviceId)
        {
            if (!AudioMMDevices.HasDefaultAudioEndpoint(dataFlow, deviceRole)) return;
            _defaultEndpointVolumeController.AudioEndpointVolume.OnVolumeNotification -= AudioEndpointVolume_OnVolumeNotification;
            _defaultEndpointVolumeController = AudioMMDevices.GetDefaultAudioEndpoint(dataFlow, deviceRole);
            DefaultAudioEndPointDeviceID = _defaultEndpointVolumeController.ID;
            _defaultEndpointVolumeController.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
        }

        public void OnDeviceAdded(string deviceId) { }

        public void OnDeviceRemoved(string deviceId) { }

        public void OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
            switch (newState)
            {
                case DeviceState.Active:
                    var device = AudioMMDevices.GetDevice(deviceId);
                    if (device.DataFlow != DataFlow.Render) return;
                    if (!AudioEndPointDeviceNames.ContainsKey(deviceId))
                    {
                        AudioEndPointDeviceNames.Add(deviceId, device.FriendlyName);
                    }
                    break;
                default:
                    if (AudioEndPointDeviceNames.ContainsKey(deviceId))
                    {
                        AudioEndPointDeviceNames.Remove(deviceId);
                    }
                    break;
            }

            if (!AudioEndPointDeviceNames.ContainsKey(DefaultAudioEndPointDeviceID))
            {
                DefaultAudioEndPointDeviceID = AudioMMDevices.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia).ID;
            }
            if (!AudioEndPointDeviceNames.ContainsKey(AudioEndPointDeviceID))
            {
                AudioEndPointDeviceID = DefaultAudioEndPointDeviceID;
            }

            FrmEspionSpotify.Instance.UpdateAudioDevicesDataSource();
        }

        public void OnPropertyValueChanged(string deviceId, PropertyKey propertyKey) { }

        public void Dispose()
        {
            Dispose(true);
 
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _defaultEndpointVolumeController.AudioEndpointVolume.OnVolumeNotification -= AudioEndpointVolume_OnVolumeNotification;
                _defaultEndpointVolumeController.Dispose();
                AudioMMDevices.Dispose();
            }

            _disposed = true;
        }
    }
}
