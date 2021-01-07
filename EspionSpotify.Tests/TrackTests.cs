using Xunit;
using EspionSpotify.Models;
using EspionSpotify.Enums;

namespace EspionSpotify.Tests
{
    public class TrackTests
    {
        [Fact]
        internal void DefaultTrack_ReturnsEmptyTrack()
        {
            var track = new Track();

            Assert.False(track.IsNormalPlaying);
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
                TitleExtended = "Live",
                TitleExtendedSeparatorType = TitleSeparatorType.Dash
            };

            Assert.True(track.IsNormalPlaying);
            Assert.Equal("Artist Name - Song Title - Live", track.ToString());
            Assert.NotEqual(track, new Track());
        }

        [Theory]
        [InlineData("A", "B", false, true, true)]
        [InlineData("A", "", false, true, false)]
        [InlineData("", "B", false, true, false)]
        [InlineData("", "", false, true, false)]
        [InlineData(null, null, false, true, false)]
        [InlineData("A", "B", true, true, false)]
        [InlineData("A", "B", false, false, false)]
        internal void IsNormal_ReturnsExpectedNormalResults(string artist, string title, bool ad, bool playing, bool expected)
        {
            var track = new Track() { Artist = artist, Title = title, Ad = ad, Playing = playing };

            Assert.Equal(expected, track.IsNormalPlaying);
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
                TitleExtended = "Live",
                TitleExtendedSeparatorType = TitleSeparatorType.Dash
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
            
            Assert.Equal(
                "Title",
                new Track
                {
                    Title = "Title",
                    Artist = "Artist",
                    TitleExtended = "Must Not Return",
                    TitleExtendedSeparatorType = TitleSeparatorType.None,
                }.ToTitleString());

            Assert.Equal(
                "Title - Remastered",
                new Track
                {
                    Title = "Title",
                    Artist = "Artist",
                    TitleExtended = "Remastered",
                    TitleExtendedSeparatorType = TitleSeparatorType.Dash,
                }.ToTitleString());

            Assert.Equal(
                "Title (Featuring Other)",
                new Track
                {
                    Title = "Title",
                    Artist = "Artist",
                    TitleExtended = "Featuring Other",
                    TitleExtendedSeparatorType = TitleSeparatorType.Parenthesis
                }.ToTitleString());

            Assert.Equal(
                "Title (Featuring Other) - Remastered",
                new Track
                {
                    Title = "Title (Featuring Other)",
                    Artist = "Artist",
                    TitleExtended = "Remastered",
                    TitleExtendedSeparatorType = TitleSeparatorType.Dash
                }.ToTitleString());
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
