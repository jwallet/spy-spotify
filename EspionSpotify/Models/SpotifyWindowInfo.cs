using EspionSpotify.Spotify;

namespace EspionSpotify.Models
{
    public class SpotifyWindowInfo
    {
        public string WindowTitle { get; set; }
        public bool IsPlaying { get; set; }

        internal bool IsTitledAd { get => SpotifyStatus.WindowTitleIsAd(WindowTitle); }
    }
}
