using EspionSpotify.MediaTags;
using EspionSpotify.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EspionSpotify.Spotify
{
    public class SpotifyStatus: ISpotifyStatus
    {
        public Track Track { get; set; }

        private string[] WindowTitleSeparator { get; }

        public SpotifyStatus(SpotifyWindowInfo spotifyWindowInfo)
        {
            WindowTitleSeparator = new[] {" - "};
            SetSongInfo(ref spotifyWindowInfo);
        }

        public Track GetTrack()
        {
            if (!Track.IsNormal()) return Track;

            var api = new LastFMAPI();
            var trackExtra = api.GetTagInfo(Track);

            if (trackExtra != null && trackExtra.Album != null)
            {
                Track = MapLastFMTrackToTrack(trackExtra);
            }
            else
            {
                var retryWithTrack = Track;
                retryWithTrack.Title = Regex.Replace(retryWithTrack.Title, @" \(.*?\)| \- .*", "");
                trackExtra = api.GetTagInfo(retryWithTrack);
                if (trackExtra != null)
                {
                    Track = MapLastFMTrackToTrack(trackExtra);
                }
            }

            return Track;
        }

        private Track MapLastFMTrackToTrack(LastFMTrack trackExtra)
        {
            return new Track(Track)
            {
                Album = trackExtra.Album?.AlbumTitle,
                AlbumPosition = trackExtra.Album?.TrackPosition,
                Genres = trackExtra.Toptags?.Tag?.Select(x => x.Name).ToArray(),
                Length = trackExtra.Duration / 1000,
                ArtExtraLargeUrl = trackExtra.Album?.ExtraLargeCoverUrl,
                ArtLargeUrl = trackExtra.Album?.LargeCoverUrl,
                ArtMediumUrl = trackExtra.Album?.MediumCoverUrl,
                ArtSmallUrl = trackExtra.Album?.SmallCoverUrl
            };
        }

        private void SetSongInfo(ref SpotifyWindowInfo spotifyWindowInfo)
        {
            var tags = spotifyWindowInfo.WindowTitle.Split(WindowTitleSeparator, 3, StringSplitOptions.None);

            var isPlaying = spotifyWindowInfo.IsPlaying || !spotifyWindowInfo.IsTitledSpotify;
            var isAd = tags.Length < 2;

            Track = new Track
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
