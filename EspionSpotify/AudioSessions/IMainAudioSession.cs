using System.Collections.Generic;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;

namespace EspionSpotify.AudioSessions
{
    public interface IMainAudioSession
    {
        MMDeviceEnumerator AudioMMDevices { get; }
        AudioMMDevicesManager AudioMMDevicesManager { get; }

        void SetSpotifyProcesses();
        void RouteSpotifyAudioSessions();
        void UnrouteSpotifyAudioSessions();
        ICollection<AudioSessionControl> SpotifyAudioSessionControls { get; }

        int AudioDeviceVolume { get; }
        bool IsAudioEndPointDeviceIndexAvailable { get; }
        void ClearSpotifyAudioSessionControls();

        void SetAudioDeviceVolume(int volume);

        Task SleepWhileTheSongEnds();
        Task<bool> IsSpotifyCurrentlyPlaying();
        void SetSpotifyToMute(bool mute);
        Task<bool> WaitSpotifyAudioSessionToStart(bool running);
        void SetSpotifyVolumeToHighAndOthersToMute(bool mute = false);

        void Dispose();
    }
}