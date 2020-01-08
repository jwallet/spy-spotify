using System.Resources;

namespace EspionSpotify
{
    public interface IFrmEspionSpotify 
    {
        ResourceManager Rm { get; }
        void UpdateIconSpotify(bool isSpotifyPlaying, bool isRecording = false);
        void UpdatePlayingTitle(string text);
        void UpdateRecordedTime(int? time);
        void UpdateStartButton();
        void StopRecording();
        void UpdateNum(int num);
        void WriteIntoConsole(string resource, params object[] args);
    }
}
