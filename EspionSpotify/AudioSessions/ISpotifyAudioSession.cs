using System.Threading.Tasks;

namespace EspionSpotify.AudioSessions
{
    public interface ISpotifyAudioSession
    {
        void SleepWhileTheSongEnds();
        bool IsSpotifyCurrentlyPlaying();
        void SetSpotifyToMute(bool mute);
        Task WaitSpotifyAudioSessionToStart();
        void SetSpotifyVolumeToHighAndOthersToMute(bool mute = false);
    }
}
