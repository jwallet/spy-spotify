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

        [Fact]
        internal void ToTitleString_ReturnsTitleAndExtendedTitle()
        { 
            Assert.Equal("", new Track().ToTitleString());
            Assert.Equal("Title", new Track { Title = "Title", Artist = "Artist", TitleExtended = "" }.ToTitleString());
            Assert.Equal("Title - Remastered", new Track { Title = "Title", Artist = "Artist", TitleExtended = "Remastered" }.ToTitleString());
        }

        [Fact]
        internal void TrackEquals_ReturnsAsExpected()
        {
            var trackEmpty = new Track();
            var trackDetailled = new Track()
            {
                Title = "Title",
                Artist = "Artist",
                TitleExtended = "",
                Ad = false,
            };

            Assert.True(trackEmpty.Equals(trackEmpty));
            Assert.True(trackEmpty.Equals(new Track()));
            Assert.True(trackDetailled.Equals(new Track { Title = "Title", Artist = "Artist" }));

            Assert.False(trackEmpty.Equals(null));
            Assert.False(trackEmpty.Equals(new OutputFile()));
            Assert.False(trackEmpty.Equals(new Track() { Title = "Title" }));
        }
    }
}
