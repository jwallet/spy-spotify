using NAudio.CoreAudioApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EspionSpotify.AudioSessions
{
    public interface ISpotifyAudioSession: IMainAudioSession
    {
        ICollection<AudioSessionControl> SpotifyAudioSessionControls { get; }
        void SleepWhileTheSongEnds();
        Task<bool> IsSpotifyCurrentlyPlaying();
        void SetSpotifyToMute(bool mute);
        Task<bool> WaitSpotifyAudioSessionToStart(bool running);
        void SetSpotifyVolumeToHighAndOthersToMute(bool mute = false);
    }
}
