using NAudio.CoreAudioApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EspionSpotify.AudioSessions
{
    public interface IMainAudioSession
    {
        MMDeviceEnumerator AudioMMDevices { get; }
        AudioMMDevicesManager AudioMMDevicesManager { get; }

        ICollection<AudioSessionControl> SpotifyAudioSessionControls { get; }
        void ClearSpotifyAudioSessionControls();

        int AudioDeviceVolume { get; }
        bool IsAudioEndPointDeviceIndexAvailable { get; }

        void SetAudioDeviceVolume(int volume);

        void SleepWhileTheSongEnds();
        Task<bool> IsSpotifyCurrentlyPlaying();
        void SetSpotifyToMute(bool mute);
        Task<bool> WaitSpotifyAudioSessionToStart(bool running);
        void SetSpotifyVolumeToHighAndOthersToMute(bool mute = false);

        void Dispose();
    }
}
