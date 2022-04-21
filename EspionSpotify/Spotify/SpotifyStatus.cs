using System;
using System.Linq;
using System.Threading.Tasks;
using EspionSpotify.API;
using EspionSpotify.Enums;
using EspionSpotify.Models;

namespace EspionSpotify.Spotify
{
    public class SpotifyStatus : ISpotifyStatus
    {
        public SpotifyStatus(SpotifyWindowInfo spotifyWindowInfo)
        {
            var tags = GetDashTags(spotifyWindowInfo.WindowTitle, 2);
            var longTitlePart = GetTitleTag(tags, 2);
            var (titleTags, separatorType) = GetTitleTags(longTitlePart ?? "", 2);

            var isPlaying = spotifyWindowInfo.IsPlaying;
            var isLookingLikeAnAd = tags.Length < 2 || tags.First() == Constants.SPOTIFY;

            CurrentTrack = new Track
            {
                Ad = spotifyWindowInfo.IsTitledAd || (isLookingLikeAnAd && isPlaying),
                Playing = isPlaying,
                Artist = GetTitleTag(tags, 1),
                Title = GetTitleTag(titleTags, 1),
                TitleExtended = GetTitleTag(titleTags, 2),
                TitleExtendedSeparatorType = separatorType
            };
        }

        public Track CurrentTrack { get; set; }

        public async Task<Track> GetTrack()
        {
            if (!CurrentTrack.IsNormalPlaying)
            {
                await ExternalAPI.Instance.Authenticate();
                return CurrentTrack;
            }

            _ = Task.Run(async () =>
            {
                await Task.Delay(1000);
                CurrentTrack.MetaDataUpdated = await ExternalAPI.Instance.UpdateTrack(CurrentTrack);
            });

            return CurrentTrack;
        }

        public static bool WindowTitleIsAd(string title)
        {
            return title?.ToLowerInvariant() == Constants.ADVERTISEMENT.ToLowerInvariant();
        }

        public static string[] GetDashTags(string title, int maxSize = 3)
        {
            return title.Split(new[] {" - "}, maxSize, StringSplitOptions.RemoveEmptyEntries);
        }

        public static (string[], TitleSeparatorType) GetTitleTags(string title, int maxSize = 2)
        {
            if (string.IsNullOrWhiteSpace(title)) return (null, TitleSeparatorType.None);

            var byDash = GetDashTags(title, maxSize);
            var byParenthesis = title.Split(new[] {" ("}, maxSize, StringSplitOptions.RemoveEmptyEntries);
            if (byParenthesis.Length == 2) byParenthesis[1] = byParenthesis[1].Replace(")", "");

            if (byDash.Length > 1) return (byDash, TitleSeparatorType.Dash);
            if (byParenthesis.Length > 1) return (byParenthesis, TitleSeparatorType.Parenthesis);

            return (new[] {title}, TitleSeparatorType.None);
        }

        public static string GetTitleTag(string[] tags, int maxValue)
        {
            return tags != null && tags.Length >= maxValue && maxValue != 0 ? tags[maxValue - 1] ?? null : null;
        }
    }
}