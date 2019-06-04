namespace EspionSpotify
{
    public interface IFrmEspionSpotify 
    {
        void UpdateIconSpotify(bool isSpotifyPlaying, bool isRecording = false);
        void UpdatePlayingTitle(string text);
        void UpdateStartButton();
        void StopRecording();
        void UpdateNum(int num);
        void WriteIntoConsole(string text);
    }
}
