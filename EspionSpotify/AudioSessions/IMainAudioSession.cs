using NAudio.CoreAudioApi;

namespace EspionSpotify.AudioSessions
{
    public interface IMainAudioSession
    {
        int? AudioEndPointDeviceIndex { get; set; }
        MMDevice DefaultEndPointDevice { get; set; }
        MMDevice AudioEndPointDevice { get; set; }
        MMDeviceCollection AudioEndPointDevices { get; set; }
        bool IsAudioEndPointDeviceIndexAvailable();
        int AudioDeviceVolume { get; set; }
        void UpdateAudioEndPointDevices(MMDeviceEnumerator aMmDevices);
        void SetDefaultAudioDeviceVolume(int volume);
    }
}
