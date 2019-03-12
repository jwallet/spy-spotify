using Xunit;
using EspionSpotify.MediaTags;
using EspionSpotify.Models;

namespace EspionSpotify.Tests
{
    public class LastFMAPITests
    {
        [Fact]
        private async void GetTagInfo_ReturnsLastFMTrack()
        {
            var spotifyTrack = new Track()
            {
                Title = "Song Title",
                Artist = "Artist Name",
                Ad = false,
                Playing = true,
                TitleExtended = "Live"
            };

            var api = new LastFMAPI();

            var lastFMTrack = api.UpdateTrack(spotifyTrack);

            Assert.True(await lastFMTrack);
        }
    }
}
