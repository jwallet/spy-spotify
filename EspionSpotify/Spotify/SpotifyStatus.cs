using EspionSpotify.MediaTags;
using EspionSpotify.Models;
using System;
using System.Linq;
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
            var api = new LastFMAPI();
            var trackExtra = api.GetTagInfo(Track);

            if (trackExtra != null)
            {
                Track.Album = trackExtra.Album?.AlbumTitle;
                Track.AlbumPosition = trackExtra.Album?.TrackPosition;
                Track.Genres = trackExtra.Toptags?.Tag?.Select(x => x.Name).ToArray();
                Track.Length = trackExtra.Duration / 1000;
                Track.ArtExtraLargeUrl = trackExtra.Album?.ExtraLargeCoverUrl;
                Track.ArtLargeUrl = trackExtra.Album?.LargeCoverUrl;
                Track.ArtMediumUrl = trackExtra.Album?.MediumCoverUrl;
                Track.ArtSmallUrl = trackExtra.Album?.SmallCoverUrl;
            }

            return Track;
        }

        private void SetSongInfo(ref SpotifyWindowInfo spotifyWindowInfo)
        {
            var tags = spotifyWindowInfo.WindowTitle.Split(WindowTitleSeparator, 3, StringSplitOptions.None);

            var isPlaying = spotifyWindowInfo.IsPlaying;
            var isAd = tags.Length < 2;

            Track = new Track
            {
                Ad = isAd && isPlaying,
                Playing = isPlaying || !spotifyWindowInfo.IsTitledSpotify(),
                Artist = GetTitleTag(tags, 1),
                Title = GetTitleTag(tags, 2),
                TitleExtended = GetTitleTag(tags, 3),
            };
        }

        private string GetTitleTag(string[] tags, int maxValue) => tags.Length >= maxValue ? tags[maxValue - 1] ?? null : null;
    }
}
