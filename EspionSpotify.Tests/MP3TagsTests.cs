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
        const string ART_LINK = "https://raw.githubusercontent.com/jwallet/spy-spotify/master/psd/spytify-espion-spotify-logo-small.png";
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
            var mp3Tags = new MP3Tags(_fileSystem)
            {
                CurrentFile = _currentFile.ToString(),
                Count = null,
                OrderNumberInMediaTagEnabled = false,
                Track = _track
            };

            var tags = new TagLibTab();
            await mp3Tags.MapMediaTags(tags);

            Assert.Equal(_track.ToString(), mp3Tags.Track.ToString());

            Assert.Equal(_track.Title, tags.Title);
            Assert.Equal(_track.Artist, tags.AlbumArtists.FirstOrDefault());
            Assert.Equal(_track.Artist, tags.Performers.FirstOrDefault());
            Assert.Null(tags.Album);
            Assert.Null(tags.Genres);
            Assert.Equal(0u, tags.Disc);
            Assert.Equal(0u, tags.Year);
            Assert.Null(tags.Pictures);
        }


        [Fact]
        internal async void TrackNumber_ReturnsTrackNumberTag()
        {
            var mp3Tags = new MP3Tags(_fileSystem)
            {
                CurrentFile = _currentFile.ToString(),
                Count = 2,
                OrderNumberInMediaTagEnabled = true,
                Track = _track
            };

            var tags = new TagLibTab();
            await mp3Tags.MapMediaTags(tags);

            Assert.Equal(_track.Title, tags.Title);
            Assert.Equal(2u, tags.Track);
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
                Disc = 1u,
                Year = 2020u,
                ArtLargeUrl = "www.google.com",
                ArtMediumUrl = ART_LINK,
                ArtSmallUrl = ART_LINK
            };

            var mp3Tags = new MP3Tags(_fileSystem)
            {
                CurrentFile = _currentFile.ToString(),
                Count = null,
                OrderNumberInMediaTagEnabled = false,
                Track = track
            };

            var tags = new TagLibTab();
            await mp3Tags.MapMediaTags(tags);

            Assert.Equal(track.ToString(), mp3Tags.Track.ToString());

            Assert.Equal(track.AlbumPosition, (int?)tags.Track);
            Assert.Equal(track.Title, tags.Title);
            Assert.Equal(track.AlbumArtists, tags.AlbumArtists);
            Assert.Equal(track.Performers, tags.Performers);
            Assert.Equal(track.Album, tags.Album);
            Assert.Equal(track.Genres, tags.Genres);
            Assert.Equal(track.Disc, tags.Disc);
            Assert.Equal(track.Year, tags.Year);

            Assert.Single(tags.Pictures);
            Assert.Equal(track.ArtMedium.Length, tags.Pictures[0].Data.Count);
            Assert.Equal(TagLib.PictureType.FrontCover, tags.Pictures[0].Type);
        }
    }
}
