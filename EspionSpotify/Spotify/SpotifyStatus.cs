using EspionSpotify.Enums;
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

        public static string[] SpotifyTitles = new[] { SPOTIFY, SPOTIFYFREE };

        public Track CurrentTrack { get; set; }

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
            var tags = GetDashTags(spotifyWindowInfo.WindowTitle, 2);
            var (titleTags, separatorType) = GetTitleTags(GetTitleTag(tags, 2) ?? "", 2);

            var isPlaying = spotifyWindowInfo.IsPlaying || !spotifyWindowInfo.IsTitledSpotify;
            var isAd = tags.Length < 2 || spotifyWindowInfo.IsTitledAd;

            CurrentTrack = new Track
            {
                Ad = isAd && isPlaying,
                Playing = isPlaying,
                Artist = GetTitleTag(tags, 1),
                Title = GetTitleTag(titleTags, 1),
                TitleExtended = GetTitleTag(titleTags, 2),
                TitleExtendedSeparatorType = separatorType,
            };
        }

        public static string[] GetDashTags(string title, int maxSize = 3)
        {
            return title.Split(new[] { $" - " }, maxSize, StringSplitOptions.RemoveEmptyEntries);
        }

        public static (string[], TitleSeparatorType) GetTitleTags(string title, int maxSize = 2)
        {
            if (string.IsNullOrWhiteSpace(title)) return (null, TitleSeparatorType.None);
            
            var byDash = GetDashTags(title, maxSize);
            var byParenthesis = title.Split(new[] { $" (" }, maxSize, StringSplitOptions.RemoveEmptyEntries);
            if (byParenthesis.Length == 2) byParenthesis[1] = byParenthesis[1].Replace(")", "");
            
            if (byDash.Length > 1)
            {
                return (byDash, TitleSeparatorType.Dash);
            }
            if (byParenthesis.Length > 1)
            {
                return (byParenthesis, TitleSeparatorType.Parenthesis);
            }

            return (new[] { title }, TitleSeparatorType.None);
        }

        public static string GetTitleTag(string[] tags, int maxValue) => tags != null && tags.Length >= maxValue && maxValue != 0 ? tags[maxValue - 1] ?? null : null;
    }
}
