using Xunit;
using EspionSpotify.Models;

namespace EspionSpotify.Tests
{
    public class TrackTests
    {
        [Fact]
        private void DefaultTrack_ReturnsEmptyTrack()
        {
            var track = new Track();

            Assert.False(track.IsNormal());
            Assert.Equal("Spotify", track.ToString());
        }

        [Fact]
        private void MinimalTrack_ReturnsBasicInfo()
        {
            var track = new Track
            {
                Title = "Song Title",
                Artist = "Artist Name",
                Ad = false,
                Playing = true,
                TitleExtended = "Live"
            };

            Assert.True(track.IsNormal());
            Assert.Equal("Artist Name - Song Title - Live", track.ToString());
            Assert.NotEqual(track, new Track());
        }

        [Fact]
        private async void TrackWithArtsLinks_ReturnsArtsData()
        {
            var link = "https://raw.githubusercontent.com/jwallet/spy-spotify/master/psd/spytify-espion-spotify-logo-small.png";

            var track = new Track
            {
                ArtSmallUrl = link,
                ArtMediumUrl = link,
                ArtLargeUrl = link,
                ArtExtraLargeUrl = link
            };

            await track.GetArtSmallAsync();
            await track.GetArtMediumAsync();
            await track.GetArtLargeAsync();
            await track.GetArtExtraLargeAsync();

            Assert.NotEmpty(track.ArtSmall);
            Assert.NotEmpty(track.ArtMedium);
            Assert.NotEmpty(track.ArtLarge);
            Assert.NotEmpty(track.ArtExtraLarge);
        }

        [Fact]
        private void TrackWithInitialTrack_ReturnsUpdatedTrack()
        {
            var initialTrack = new Track
            {
                Title = "Song Title",
                Artist = "Artist Name",
                Ad = false,
                Playing = true,
                TitleExtended = "Live"
            };

            var track = new Track(initialTrack)
            {
                Album = "Album",
                AlbumPosition = 1
            };

            Assert.Equal(initialTrack.ToString(), track.ToString());
            Assert.NotEqual(initialTrack.Album, track.Album);
            Assert.NotEqual(track, new Track());
        }
    }
}
