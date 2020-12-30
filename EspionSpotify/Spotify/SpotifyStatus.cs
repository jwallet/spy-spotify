using EspionSpotify.Enums;
using EspionSpotify.API;
using EspionSpotify.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EspionSpotify.Spotify
{
    public class SpotifyStatus : ISpotifyStatus
    {
        public static string[] SpotifyTitles = new[] { Constants.SPOTIFY.ToLowerInvariant(), Constants.SPOTIFYFREE.ToLowerInvariant() };

        public Track CurrentTrack { get; set; }

        public static bool WindowTitleIsSpotify(string title)
        {
            return SpotifyTitles.Contains(title?.ToLowerInvariant());
        }

        public static bool WindowTitleIsAd(string title)
        {
            return title?.ToLowerInvariant() == Constants.ADVERTISEMENT.ToLowerInvariant();
        }

        public async Task<Track> GetTrack()
        {
            if (!CurrentTrack.IsNormal)
            {
                await ExternalAPI.Instance.Authenticate();
                return CurrentTrack;
            }

            _ = Task.Run(async () =>
            {
                await Task.Delay(1000);
                await ExternalAPI.Instance.UpdateTrack(CurrentTrack);
            });

            return CurrentTrack;
        }

        public SpotifyStatus(SpotifyWindowInfo spotifyWindowInfo)
        {
            var tags = SpotifyStatus.GetDashTags(spotifyWindowInfo.WindowTitle, 2);
            var longTitlePart = SpotifyStatus.GetTitleTag(tags, 2);
            var (titleTags, separatorType) = SpotifyStatus.GetTitleTags(longTitlePart ?? "", 2);

            var isPlaying = spotifyWindowInfo.IsPlaying;
            var isAd = tags.Length < 2 || spotifyWindowInfo.IsTitledAd || tags.First() == Constants.SPOTIFY;

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
