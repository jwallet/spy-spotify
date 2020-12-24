using EspionSpotify.MediaTags;
using EspionSpotify.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EspionSpotify.Spotify
{
    public class SpotifyStatus: ISpotifyStatus
    {
        public const string SPOTIFY = "spotify";
        public const string SPOTIFYFREE = "spotify free";
        public const string ADVERTISEMENT = "advertisement";
        public const string TITLE_SEPARATOR = " - ";

        public static string[] SpotifyTitles = new[] { SPOTIFY, SPOTIFYFREE };

        public Track CurrentTrack { get; set; }

        private string[] _windowTitleSeparator { get; }

        public static bool WindowTitleIsSpotify(string title)
        {
            return SpotifyTitles.Contains(title?.ToLowerInvariant());
        }

        public static bool WindowTitleIsAd(string title)
        {
            return title?.ToLowerInvariant() == ADVERTISEMENT;
        }

        public SpotifyStatus(SpotifyWindowInfo spotifyWindowInfo)
        {
            _windowTitleSeparator = new[] { TITLE_SEPARATOR };
            SetSongInfo(ref spotifyWindowInfo);
        }

        public Track GetTrack()
        {
            if (!CurrentTrack.IsNormal) return CurrentTrack;
            
            _ = Task.Run(async () =>
            {
                await Task.Delay(1000);
                await ExternalAPI.Instance.UpdateTrack(CurrentTrack);
            });

            return CurrentTrack;
        }

        private void SetSongInfo(ref SpotifyWindowInfo spotifyWindowInfo)
        {
            var tags = GetTags(spotifyWindowInfo.WindowTitle);

            var isPlaying = spotifyWindowInfo.IsPlaying || !spotifyWindowInfo.IsTitledSpotify;
            var isAd = tags.Length < 2 || spotifyWindowInfo.IsTitledAd;

            CurrentTrack = new Track
            {
                Ad = isAd && isPlaying,
                Playing = isPlaying,
                Artist = GetTitleTag(tags, 1),
                Title = GetTitleTag(tags, 2),
                TitleExtended = GetTitleTag(tags, 3),
            };
        }

        public static string[] GetTags(string title, int maxSize = 3) => title.Split(new[] { TITLE_SEPARATOR }, maxSize, StringSplitOptions.None);

        public static string GetTitleTag(string[] tags, int maxValue) => tags.Length >= maxValue && maxValue != 0 ? tags[maxValue - 1] ?? null : null;
    }
}
