using EspionSpotify.Enums;
using EspionSpotify.MediaTags;
using EspionSpotify.Models;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EspionSpotify.Tests
{
    public class MP3TagsTests
    {
        const string SPOTIFY_LOGO_EN_LINK = "https://raw.githubusercontent.com/jwallet/spy-spotify/master/psd/logo-en.png";
        const string SPOTIFY_LOGO_FR_LINK = "https://raw.githubusercontent.com/jwallet/spy-spotify/master/psd/logo-fr.png";
        private readonly IFileSystem _fileSystem;
        private readonly Track _track;
        private readonly OutputFile _currentFile;

        public MP3TagsTests()
        {
            _track = new Track()
            {
                Artist = "Artist",
                Title = "Song"
            };

            _currentFile = new OutputFile()
            {
                Extension = MediaFormat.Mp3.ToString(),
                File = _track.ToString(),
                Path = @"C:\path",
                Separator = "_",
            };

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\path\Artist", new MockDirectoryData() },
                { _currentFile.ToString(), new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });
        }

        [Fact]
        internal async void DefaultTrack_ReturnsNoTags()
        {
            var mapper = new MapperID3(_track, false);
            
            var tags = await mapper.GetTags();

            Assert.Equal(_track.ToString(), tags.Track.ToString());

            Assert.Equal(_track.Title, tags.Title);
            Assert.Equal(_track.Artist, tags.AlbumArtist);
            Assert.Equal(_track.Artist, tags.Artist);
            Assert.Null(tags.Album);
            Assert.Null(tags.Genre);
            Assert.Null(tags.Year);
            Assert.Null(tags.AlbumArt);
        }


        [Fact]
        internal async void TrackNumber_ReturnsTrackNumberTag()
        {
            var mapper = new MapperID3(_track, true, 2);

            var tags = await mapper.GetTags();

            Assert.Equal(_track.Title, tags.Title);
            Assert.Equal("2", tags.Track);
        }

        [Fact]
        internal async void APITrack_ReturnsPartTags()
        {
            var track = new Track
            {
                Artist = "Artist",
                Title = "Song",
                AlbumPosition = 1,
                AlbumArtists = new[] { "Alpha", "Bravo", "Charlie" },
                Performers = new[] { "Delta", "Echo", "Foxtrot" },
                Album = "Golf",
                Genres = new[] { "Hotel", "India", "Juliet" },
                Year = 2020,
                ArtLargeUrl = "www.google.com",
                ArtMediumUrl = SPOTIFY_LOGO_EN_LINK,
                ArtSmallUrl = SPOTIFY_LOGO_FR_LINK
            };

            var mapper = new MapperID3(track, false);

            var tags = await mapper.GetTags();

            Assert.Equal(track.ToString(), tags.Track.ToString());

            Assert.Equal(track.AlbumPosition.ToString(), tags.Track);
            Assert.Equal(track.Title, tags.Title);
            Assert.Equal(string.Join(", ", track.AlbumArtists), tags.AlbumArtist);
            Assert.Equal(string.Join(", ", track.Performers), tags.Artist);
            Assert.Equal(track.Album, tags.Album);
            Assert.Equal(string.Join("/", track.Genres), tags.Genre);
            Assert.Equal(track.Year.ToString(), tags.Year);

            Assert.Equal(track.ArtMedium.Length, tags.AlbumArt.Length);
        }
    }
}
