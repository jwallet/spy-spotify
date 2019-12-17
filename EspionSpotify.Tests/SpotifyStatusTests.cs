using EspionSpotify.Models;
using EspionSpotify.Spotify;
using Xunit;

namespace EspionSpotify.Tests
{
    public class SpotifyStatusTests
    {
        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("Artist Name - Song Title", false)]
        [InlineData("Spotify Ad", false)]
        [InlineData("Spotify", true)]
        [InlineData("SPOTIFY", true)]
        [InlineData("spotify", true)]
        [InlineData("Spotify Free", true)]
        private void SpotifyStatusWindowTitleIsSpotify_ReturnsWhenItMatches(string value, bool expected)
        {
            var actual = SpotifyStatus.WindowTitleIsSpotify(value);

            Assert.Equal(expected, actual);
        }

        [Fact]
        private void SpotifyStatusSpotifyStandingBy_ReturnsExpectingTrack()
        {
            var expectedTrack = new Track
            {
                Artist = "Spotify"
            };
            var spotifyWindowInfo = new SpotifyWindowInfo
            {
                WindowTitle = "Spotify",
                IsPlaying = false
            };

            var status = new SpotifyStatus(spotifyWindowInfo);

            Assert.Equal(expectedTrack, status.CurrentTrack);
            Assert.Equal("Spotify", status.CurrentTrack.ToString());
        }

        [Theory]
        [InlineData("Artist Name - Song Title", false)]
        [InlineData("Artist Name - Song Title - Live", true)]
        private void SpotifyStatusTrackPlaying_ReturnsExpectingTrack(string windowTitle, bool isTitleExtended)
        {
            var expectedTrack = new Track
            {
                Artist = "Artist Name",
                Title = "Song Title",
                TitleExtended = isTitleExtended ? "Live" : "",
                Playing = true,
                Ad = false
            };
            var spotifyWindowInfo = new SpotifyWindowInfo
            {
                WindowTitle = windowTitle,
                IsPlaying = true
            };

            var status = new SpotifyStatus(spotifyWindowInfo);

            Assert.Equal(expectedTrack, status.CurrentTrack);
            Assert.Equal(windowTitle, status.CurrentTrack.ToString());
        }

        [Theory]
        [InlineData("Spotify")]
        [InlineData("Spotify Sponsor")]
        [InlineData("PODCAST #1337: DAILY NEWS")]
        private void SpotifyStatusSpotifyPlayingAdOrUnknown_ReturnsExpectingTrack(string windowTitle)
        {
            var expectedTrack = new Track
            {
                Artist = windowTitle,
                Title = null,
                TitleExtended = null,
                Playing = true,
                Ad = true
            };
            var spotifyWindowInfo = new SpotifyWindowInfo
            {
                WindowTitle = windowTitle,
                IsPlaying = true
            };

            var status = new SpotifyStatus(spotifyWindowInfo);

            Assert.Equal(expectedTrack, status.CurrentTrack);
            Assert.Equal(expectedTrack.ToString(), status.CurrentTrack.ToString());
        }
    }
}
