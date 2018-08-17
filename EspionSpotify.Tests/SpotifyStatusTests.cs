using EspionSpotify.Models;
using EspionSpotify.Spotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EspionSpotify.Tests
{
    public class SpotifyStatusTests
    {
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

            Assert.Equal(expectedTrack, status.Track);
            Assert.Equal("Spotify", status.Track.ToString());
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

            Assert.Equal(expectedTrack, status.Track);
            Assert.Equal(windowTitle, status.Track.ToString());
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

            Assert.Equal(expectedTrack, status.Track);
            Assert.Equal("Spotify - Ad", status.Track.ToString());
        }
    }
}
