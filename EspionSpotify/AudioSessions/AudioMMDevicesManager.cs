using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using EspionSpotify.Extensions;

namespace EspionSpotify.AudioSessions
{
    public sealed class AudioMMDevicesManager : NAudio.CoreAudioApi.Interfaces.IMMNotificationClient, IDisposable
    {
        private bool _disposed = false;
        private MMDevice _defaultEndpointVolumeController;

        internal bool VolumeNotificationEmitted = false;

        public MMDeviceEnumerator AudioMMDevices { get; private set; }
        public string AudioEndPointDeviceID { get; private set; }
        public string DefaultAudioEndPointDeviceID { get; private set; }

        public bool? AudioEndPointDeviceMute => AudioEndPointDevice?.AudioEndpointVolume?.Mute;

        public string AudioEndPointDeviceName => AudioEndPointDevice?.GetFriendlyName();

        public IDictionary<string, string> AudioEndPointDeviceNames { get; private set; }

        public SessionCollection GetAudioEndPointDeviceSessions => AudioEndPointDevice?.AudioSessionManager?.Sessions;

        public MMDevice AudioEndPointDevice
        {
            get
            {
                var deviceName = AudioEndPointDeviceNames.IncludesKey(AudioEndPointDeviceID)
                    ? AudioEndPointDeviceID
                    : DefaultAudioEndPointDeviceID;
                return deviceName != null && AudioMMDevices != null ? AudioMMDevices.GetDeviceSafeException(deviceName, safe: true) : null;
            }
        }

        #region AudioSession default endpoint
        private MMDevice GetDefaultAudioEndpoint(DataFlow dataFlow, Role deviceRole)
        {
            if (AudioMMDevices == null) return null;
            if (!AudioMMDevices.HasDefaultAudioEndpoint(dataFlow, deviceRole)) return null;
            return AudioMMDevices.GetDefaultAudioEndpointSafeException(DataFlow.Render, Role.Multimedia, safe: !_disposed);
        }

        private void SetDefaultAudioEndpoint(DataFlow dataFlow, Role deviceRole)
        {
            _defaultEndpointVolumeController = GetDefaultAudioEndpoint(dataFlow, deviceRole);

            if (_defaultEndpointVolumeController == null) return;

            DefaultAudioEndPointDeviceID = _defaultEndpointVolumeController?.ID;
            if (DefaultAudioEndPointDeviceID != null)
            {
                _defaultEndpointVolumeController.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
            }
        }
        #endregion AudioSession default endpoint

        public AudioMMDevicesManager(MMDeviceEnumerator audioMMDevices, string audioEndpointDeviceID)
        {
            AudioMMDevices = audioMMDevices;
            AudioEndPointDeviceID = audioEndpointDeviceID;

            AudioEndPointDeviceNames = AudioMMDevices
                .EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)
                .Where(x => x.GetFriendlyName() != null)
                .Select(x => new KeyValuePair<string, string>(x.ID, x.FriendlyName))
                .ToDictionary(o => o.Key, o => o.Value);

            SetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
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
            VolumeNotificationEmitted = true;
        }

        public void OnDefaultDeviceChanged(DataFlow dataFlow, Role deviceRole, string defaultDeviceId)
        {
            if (AudioMMDevices == null) return;
            if (!AudioMMDevices.HasDefaultAudioEndpoint(dataFlow, deviceRole)) return;
            
            if (DefaultAudioEndPointDeviceID != null && _defaultEndpointVolumeController != null)
            {
                _defaultEndpointVolumeController.AudioEndpointVolume.OnVolumeNotification -= AudioEndpointVolume_OnVolumeNotification;
            }
            
            SetDefaultAudioEndpoint(dataFlow, deviceRole);
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
                    if (!AudioEndPointDeviceNames.IncludesKey(deviceId))
                    {
                        AudioEndPointDeviceNames.Add(deviceId, device.GetFriendlyName());
                    }
                    break;
                case DeviceState.Disabled:
                case DeviceState.NotPresent:
                case DeviceState.Unplugged:
                case DeviceState.All:
                default:
                    if (AudioEndPointDeviceNames.IncludesKey(deviceId))
                    {
                        AudioEndPointDeviceNames.Remove(deviceId);
                    }
                    break;
            }

            if (DefaultAudioEndPointDeviceID == null
                || !AudioEndPointDeviceNames.IncludesKey(DefaultAudioEndPointDeviceID))
            {
                DefaultAudioEndPointDeviceID = GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia)?.ID;
            }

            if (!AudioEndPointDeviceNames.IncludesKey(AudioEndPointDeviceID))
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

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _defaultEndpointVolumeController.AudioEndpointVolume.OnVolumeNotification -= AudioEndpointVolume_OnVolumeNotification;
                _defaultEndpointVolumeController.Dispose();
                AudioMMDevices.Dispose();
                AudioMMDevices = null;
            }

            _disposed = true;
        }
    }
}
