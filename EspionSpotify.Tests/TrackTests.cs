using Xunit;
using EspionSpotify.Models;

namespace EspionSpotify.Tests
{
    public class TrackTests
    {
        [Fact]
        internal void DefaultTrack_ReturnsEmptyTrack()
        {
            var track = new Track();

            Assert.False(track.IsNormal());
            Assert.Equal("Spotify", track.ToString());
        }

        [Fact]
        internal void MinimalTrack_ReturnsBasicInfo()
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
        internal void TrackWithInitialTrack_ReturnsUpdatedTrack()
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
                AlbumPosition = 1,
                ArtExtraLargeUrl = "http://logo.png",
            };

            Assert.Equal(initialTrack.ToString(), track.ToString());
            Assert.NotEqual(initialTrack.Album, track.Album);
            Assert.NotEqual(initialTrack.ArtExtraLargeUrl, track.ArtExtraLargeUrl);
            Assert.NotEqual(track, new Track());
        }
    }
}
