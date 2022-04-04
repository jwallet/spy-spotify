using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using EspionSpotify.API;
using EspionSpotify.Enums;
using EspionSpotify.Models;
using Xunit;

namespace EspionSpotify.Tests
{
    public class LastFMAPITests
    {
        private readonly ILastFMAPI _lastFMAPI;
        private readonly Track _track;

        public LastFMAPITests()
        {
            _lastFMAPI = new LastFMAPI();
            _track = new Track {Artist = "Artist", Title = "Title"};
        }

        [Fact]
        internal async Task TestAPIKeys_ReturnsOk()
        {
            const string artist = "artist";
            const string title = "title";
            foreach (var key in _lastFMAPI.ApiKeys)
            {
                var request =
                    WebRequest.Create(
                        $"http://ws.audioscrobbler.com/2.0/?method=track.getInfo&api_key={key}&artist={artist}&track={title}");
                using (var response = await request.GetResponseAsync())
                {
                    var httpResponse = response as HttpWebResponse;
                    Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
                    response.Dispose();
                }
            }
        }

        [Fact]
        internal void MapLastFMAPIEmptyTrackToTrack_ReturnsExpectedTrack()
        {
            var previousTrack = new Track(_track);
            var trackExtra = new LastFMTrack();

            _lastFMAPI.MapLastFMTrackToTrack(_track, trackExtra);

            Assert.NotNull(_track.Title);
            Assert.Equal(previousTrack, _track);
        }

        [Fact]
        internal void MapLastFMAPITrackToTrack_OverwritesSpytifyTrack()
        {
            var trackExtra = new LastFMTrack
            {
                Name = "Do not want updated Title from this api",
                Artist = new Artist {Name = "Do not want updated Artist from this api"}
            };

            _lastFMAPI.MapLastFMTrackToTrack(_track, trackExtra);

            Assert.Equal("Title", _track.Title);
            Assert.Equal("Artist", _track.Artist);
            Assert.NotEqual(trackExtra.Name, _track.Title);
            Assert.NotEqual(trackExtra.Artist.Name, _track.Artist);
        }

        [Fact]
        internal void MapLastFMAPITrackToTrack_KeepSpytifyTrackIfEmpty()
        {
            var trackExtra = new LastFMTrack
            {
                Name = "",
                Artist = new Artist {Name = ""}
            };

            _lastFMAPI.MapLastFMTrackToTrack(_track, trackExtra);

            Assert.Equal("Title", _track.Title);
            Assert.Equal("Artist", _track.Artist);
        }

        [Fact]
        internal void MapLastFMAPITrackToTrack_ReturnsExpectedDetailedTrack()
        {
            var trackExtra = new LastFMTrack
            {
                Name = "Do not want updated Title from this api",
                Artist = new Artist {Name = "Do not want updated Artist from this api"},
                Album = new Album
                {
                    Title = "Album Title",
                    Position = "5",
                    Image = new List<Image>
                    {
                        new Image {Size = AlbumCoverSize.extralarge.ToString(), Url = "http://xlarge-cover-url.local"},
                        new Image {Size = AlbumCoverSize.large.ToString(), Url = "http://large-cover-url.local"},
                        new Image {Size = AlbumCoverSize.medium.ToString(), Url = "http://medium-cover-url.local"},
                        new Image {Size = AlbumCoverSize.small.ToString(), Url = "http://small-cover-url.local"}
                    }
                },
                Toptags = new Toptags
                {
                    Tag = new List<Tag>
                    {
                        new Tag {Name = "Reggae", Url = "http://reggae-tag.local"},
                        new Tag {Name = "Rock", Url = "http://rock-tag.local"},
                        new Tag {Name = "Jazz", Url = "http://jazz-tag.local"}
                    }
                },
                Duration = 1337000
            };

            _lastFMAPI.MapLastFMTrackToTrack(_track, trackExtra);

            Assert.NotEqual(trackExtra.Name, _track.Title);
            Assert.NotEqual(trackExtra.Artist.Name, _track.Artist);
            Assert.Equal(trackExtra.Album.Title, _track.Album);
            Assert.Equal(5, _track.AlbumPosition);
            Assert.Equal(new[] {"Reggae", "Rock", "Jazz"}, _track.Genres);
            Assert.Equal(1337, _track.Length);
            Assert.Equal("http://xlarge-cover-url.local", _track.ArtExtraLargeUrl);
            Assert.Equal("http://large-cover-url.local", _track.ArtLargeUrl);
            Assert.Equal("http://medium-cover-url.local", _track.ArtMediumUrl);
            Assert.Equal("http://small-cover-url.local", _track.ArtSmallUrl);
        }

        [Fact]
        internal void MapLastFMAPTrackToTrack_ReturnsExpectedMissingInfoTrack()
        {
            var track = new Track
            {
                Artist = "Artist",
                Title = "Title",
                TitleExtended = "Live",
                TitleExtendedSeparatorType = TitleSeparatorType.Dash
            };

            var trackExtra = new LastFMTrack
            {
                Artist = new Artist(),
                Album = new Album
                {
                    Image = new List<Image>
                    {
                        null,
                        null,
                        null,
                        null
                    }
                },
                Toptags = new Toptags
                {
                    Tag = new List<Tag>
                    {
                        null,
                        null,
                        null
                    }
                }
            };

            _lastFMAPI.MapLastFMTrackToTrack(track, trackExtra);

            Assert.Equal("Artist - Title - Live", track.ToString());
            Assert.Equal("Title", track.Title);
            Assert.Equal("Artist", track.Artist);
            Assert.Equal(trackExtra.Album.Title, _track.Album);
            Assert.Null(track.Album);
            Assert.Null(track.AlbumPosition);
            Assert.Equal(new string[] { }, track.Genres);
            Assert.Null(track.Length);
            Assert.Null(track.ArtExtraLargeUrl);
            Assert.Null(track.ArtLargeUrl);
            Assert.Null(track.ArtMediumUrl);
            Assert.Null(track.ArtSmallUrl);
        }
    }
}