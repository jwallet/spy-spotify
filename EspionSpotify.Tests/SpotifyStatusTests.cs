using EspionSpotify.Extensions;
using EspionSpotify.Models;
using EspionSpotify.Spotify;
using Xunit;

namespace EspionSpotify.Tests
{
    public class SpotifyStatusTests
    {
        [Theory]
        [InlineData(null, true)]
        [InlineData("", true)]
        [InlineData("Artist Name - Song Title", false)]
        [InlineData(Constants.ADVERTISEMENT, false)]
        [InlineData(Constants.SPOTIFY, true)]
        [InlineData(Constants.SPOTIFYFREE, true)]
        [InlineData(Constants.SPOTIFYPREMIUM, true)]
        [InlineData("SPOTIFY", true)]
        [InlineData("spotify", true)]
        internal void SpotifyStatusWindowTitleIsSpotify_ReturnsWhenItMatches(string value, bool expected)
        {
            var actual = value.IsNullOrSpotifyIdleState();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(Constants.SPOTIFY, false)]
        [InlineData("Ad", false)]
        [InlineData(Constants.ADVERTISEMENT, true)]
        internal void SpotifyStatusWindowTitleIsAd_ReturnsWhenItMatches(string value, bool expected)
        {
            var actual = SpotifyStatus.WindowTitleIsAd(value);

            Assert.Equal(expected, actual);
        }

        [Fact]
        internal void SpotifyStatusSpotifyStandingBy_ReturnsExpectingTrack()
        {
            var expectedTrack = new Track
            {
                Artist = Constants.SPOTIFY
            };
            var spotifyWindowInfo = new SpotifyWindowInfo
            {
                WindowTitle = Constants.SPOTIFY,
                IsPlaying = false
            };

            var status = new SpotifyStatus(spotifyWindowInfo);

            Assert.Equal(expectedTrack, status.CurrentTrack);
            Assert.Equal(Constants.SPOTIFY, status.CurrentTrack.ToString());
        }

        [Theory]
        [InlineData("Artist Name - Song Title", false)]
        [InlineData("Artist Name - Song Title - Live", true)]
        internal void SpotifyStatusTrackPlaying_ReturnsExpectingTrack(string windowTitle, bool isTitleExtended)
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
        [InlineData(Constants.SPOTIFY)]
        [InlineData("Spotify Sponsor")]
        [InlineData("#1337: DAILY NEWS")]
        internal void SpotifyStatusSpotifyPlayingAdOrUnknown_ReturnsExpectingTrack(string windowTitle)
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