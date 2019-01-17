using NAudio.CoreAudioApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EspionSpotify.AudioSessions
{
    public interface ISpotifyAudioSession: IMainAudioSession
    {
        ICollection<AudioSessionControl> SpotifyAudioSessionControls { get; }
        void SleepWhileTheSongEnds();
        bool IsSpotifyCurrentlyPlaying();
        void SetSpotifyToMute(bool mute);
        bool WaitSpotifyAudioSessionToStart(ref bool running);
        void SetSpotifyVolumeToHighAndOthersToMute(bool mute = false);
    }
}
