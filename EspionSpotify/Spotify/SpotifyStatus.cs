using EspionSpotify.MediaTags;
using EspionSpotify.Models;
using System;

namespace EspionSpotify.Spotify
{
    public class SpotifyStatus: ISpotifyStatus
    {
        public const string SPOTIFY = "spotify";

        public Track CurrentTrack { get; set; }

        private string[] _windowTitleSeparator { get; }

        public static bool WindowTitleIsSpotify(string title)
        {
            return title?.ToLowerInvariant().Equals(SPOTIFY) ?? false;
        }

        public SpotifyStatus(SpotifyWindowInfo spotifyWindowInfo)
        {
            _windowTitleSeparator = new[] {" - "};
            SetSongInfo(ref spotifyWindowInfo);
        }

        public Track GetTrack()
        {
            if (!CurrentTrack.IsNormal()) return CurrentTrack;

            ExternalAPI.Instance.UpdateTrack(CurrentTrack);
            return CurrentTrack;
        }

        private void SetSongInfo(ref SpotifyWindowInfo spotifyWindowInfo)
        {
            var tags = spotifyWindowInfo.WindowTitle.Split(_windowTitleSeparator, 3, StringSplitOptions.None);

            var isPlaying = spotifyWindowInfo.IsPlaying || !spotifyWindowInfo.IsTitledSpotify;
            var isAd = tags.Length < 2;

            CurrentTrack = new Track
            {
                Ad = isAd && isPlaying,
                Playing = isPlaying,
                Artist = GetTitleTag(tags, 1),
                Title = GetTitleTag(tags, 2),
                TitleExtended = GetTitleTag(tags, 3),
            };
        }

        private string GetTitleTag(string[] tags, int maxValue) => tags.Length >= maxValue ? tags[maxValue - 1] ?? null : null;
    }
}
