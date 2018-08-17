using System;
using Xunit;
using Moq;
using EspionSpotify;
using SpotifyAPI.Local.Models;

namespace EspionSpotify.UnitTests
{
    public class SongTests
    {
        Mock<TrackResourceLocation> location;

        Mock<SpotifyResource> albumResource;
        Mock<SpotifyResource> artistResource;
        Mock<SpotifyResource> trackResource;

        Mock<Track> track;

        private const string _uri = "https://open.spotify.com/object/unIQu3Id3nf1c4TIoNnUmB3R";
        private const string _albumTitle = "Album Title";
        private const string _artistName = "Artist Name";
        private const string _trackTitle = "Track Title";
        private const string _trackNormal = "normal";

        public SongTests()
        {
            
        }

        [Fact]
        public void Song_WithoutParameters_ReturnsDefaultSong()
        {
            var song = new Song();

            Assert.Null(song.Album);
            Assert.Null(song.Artist);
            Assert.Null(song.Title);

            Assert.Null(song.Length);
            Assert.Equal(0.0, song.CurrentLength, 0);
            Assert.Equal(_trackNormal, song.Type);

            Assert.False(song.IsAd);
            Assert.False(song.IsOther);
            Assert.True(song.IsNormal);

            Assert.Empty(song.ArtLarge);
            Assert.Empty(song.ArtExtraLarge);
        }

        [Fact]
        public void Song_WithNormalTrack_ReturnsSong()
        {
            location = new Mock<TrackResourceLocation>();
            location.Setup(x => x.Og).Returns(_uri);

            albumResource = new Mock<SpotifyResource>();
            albumResource.Setup(x => x.Name).Returns(_albumTitle);
            albumResource.Setup(x => x.Location).Returns(location.Object);

            artistResource = new Mock<SpotifyResource>();
            artistResource.Setup(x => x.Name).Returns(_artistName);
            artistResource.Setup(x => x.Location).Returns(location.Object);

            trackResource = new Mock<SpotifyResource>();
            trackResource.Setup(x => x.Name).Returns(_trackTitle);
            trackResource.Setup(x => x.Location).Returns(location.Object);

            track = new Mock<Track>();
            track.Setup(x => x.AlbumResource).Returns(albumResource.Object);
            track.Setup(x => x.ArtistResource).Returns(artistResource.Object);
            track.Setup(x => x.TrackResource).Returns(trackResource.Object);
            track.Setup(x => x.TrackType).Returns(_trackNormal);
            track.Setup(x => x.Length).Returns(420);
            track.Setup(x => x.IsAd()).Returns(false);
            track.Setup(x => x.IsOtherTrackType()).Returns(false);

            var song = new Song(track.Object);

            Assert.Equal(_albumTitle, song.Album);
            Assert.Equal(_artistName, song.Artist);
            Assert.Equal(_trackTitle, song.Title);

            Assert.Equal(420, song.Length);
            Assert.Equal(0.0, song.CurrentLength, 0);
            Assert.Empty(song.Type);

            Assert.False(song.IsAd);
            Assert.False(song.IsOther);
            Assert.False(song.IsNormal);

            Assert.Empty(song.ArtLarge);
            Assert.Empty(song.ArtExtraLarge);
        }
    }
}
