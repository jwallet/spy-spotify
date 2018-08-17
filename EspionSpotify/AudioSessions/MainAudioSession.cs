using NAudio.CoreAudioApi;

namespace EspionSpotify.AudioSessions
{
    public class MainAudioSession : IMainAudioSession
    {
        public MMDevice DefaultAudioEndPointDevice { get; set; }
        public int DefaultAudioDeviceVolume { get; set; }

        public MainAudioSession()
        {
            var aMmDevices = new MMDeviceEnumerator();
            UpdateDefaultAudioEndPointDevice(aMmDevices);

            DefaultAudioDeviceVolume = (int)(DefaultAudioEndPointDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
        }

        public void UpdateDefaultAudioEndPointDevice(MMDeviceEnumerator aMmDevices)
        {
            DefaultAudioEndPointDevice = aMmDevices.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        }

        public void SetDefaultAudioDeviceVolume(int volume)
        {
            if (float.TryParse(volume.ToString(), out var fNewVolume))
            {
                DefaultAudioEndPointDevice.AudioEndpointVolume.MasterVolumeLevelScalar = fNewVolume / 100;
            }
        }
    }
}
