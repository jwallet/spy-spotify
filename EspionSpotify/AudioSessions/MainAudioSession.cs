using NAudio.CoreAudioApi;

namespace EspionSpotify.AudioSessions
{
    public class MainAudioSession : IMainAudioSession
    {
        public int? AudioEndPointDeviceIndex { get; set; }
        public MMDevice AudioEndPointDevice { get; set; }
        public MMDevice DefaultEndPointDevice { get; set; }
        public MMDeviceCollection AudioEndPointDevices { get; set; }
        public int AudioDeviceVolume { get; set; }

        public MainAudioSession() { }

        public MainAudioSession(int? audioEndPointDeviceIndex)
        {
            AudioEndPointDeviceIndex = audioEndPointDeviceIndex;

            var aMmDevices = new MMDeviceEnumerator();
            UpdateAudioEndPointDevices(aMmDevices);
        }

        public void UpdateAudioEndPointDevices(MMDeviceEnumerator aMmDevices)
        {
            DefaultEndPointDevice = aMmDevices.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            AudioEndPointDevices = aMmDevices.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            AudioEndPointDevice = AudioEndPointDeviceIndex.HasValue ? AudioEndPointDevices[AudioEndPointDeviceIndex.Value] : DefaultEndPointDevice;
            AudioDeviceVolume = (int)(AudioEndPointDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
        }

        public void SetDefaultAudioDeviceVolume(int volume)
        {
            if (float.TryParse(volume.ToString(), out var fNewVolume))
            {
                AudioEndPointDevice.AudioEndpointVolume.MasterVolumeLevelScalar = fNewVolume / 100;
            }
        }
    }
}
