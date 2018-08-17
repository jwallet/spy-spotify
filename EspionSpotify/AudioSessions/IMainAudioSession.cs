using NAudio.CoreAudioApi;

namespace EspionSpotify.AudioSessions
{
    public interface IMainAudioSession
    {
        MMDevice DefaultAudioEndPointDevice { get; set; }
        int DefaultAudioDeviceVolume { get; set; }
        void UpdateDefaultAudioEndPointDevice(MMDeviceEnumerator aMmDevices);
        void SetDefaultAudioDeviceVolume(int volume);
    }
}
