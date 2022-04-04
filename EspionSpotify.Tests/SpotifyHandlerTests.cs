using System.Threading.Tasks;
using EspionSpotify.Events;
using EspionSpotify.Models;
using EspionSpotify.Spotify;
using Moq;
using Xunit;

namespace EspionSpotify.Tests
{
    public class SpotifyHandlerTests
    {
        [Fact]
        internal void SpotifyHandler_ReturnsSpotifyProcess()
        {
            var spotifyProcessMock = new Mock<ISpotifyProcess>();

            var spotifyHandler = new SpotifyHandler(spotifyProcessMock.Object)
            {
                ListenForEvents = true
            };

            Assert.True(spotifyHandler.ListenForEvents);
            Assert.Same(spotifyProcessMock.Object, spotifyHandler.SpotifyProcess);
        }

        [Fact]
        internal void Dispose_ReturnsTimerOff()
        {
            var spotifyProcessMock = new Mock<ISpotifyProcess>();

            var spotifyHandler = new SpotifyHandler(spotifyProcessMock.Object);
            spotifyHandler.Dispose();

            Assert.Null(spotifyHandler.EventTimer);
            Assert.Null(spotifyHandler.SongTimer);
        }

        [Fact]
        internal void TickEventSpotifyIdling_ReturnsNoEvent()
        {
            var spotifyProcessMock = new Mock<ISpotifyProcess>();

            var spotifyHandler = new SpotifyHandler(spotifyProcessMock.Object)
            {
                ListenForEvents = true
            };
            spotifyHandler.SongTimer.Start();

            var eventPlaying = false;
            spotifyHandler.OnPlayStateChange += delegate(object sender, PlayStateEventArgs e)
            {
                eventPlaying = e.Playing;
                Assert.False(eventPlaying);
            };

            Track eventNewTrack = null;
            Track eventOldTrack = null;
            spotifyHandler.OnTrackChange += delegate(object sender, TrackChangeEventArgs e)
            {
                eventNewTrack = e.NewTrack;
                eventOldTrack = e.OldTrack;
                Assert.Null(eventNewTrack);
                Assert.Null(eventOldTrack);
            };

            int? eventTrackTime = null;
            spotifyHandler.OnTrackTimeChange += delegate(object sender, TrackTimeChangeEventArgs e)
            {
                eventTrackTime = e.TrackTime;
                Assert.Equal(0, eventTrackTime);
            };

            spotifyHandler.Dispose();
        }

        [Fact]
        internal async Task TriggerEvents_ReturnsInitialSpotifyTrack()
        {
            var spotifyPaused = new Track();

            var spotifyStatusMock = new Mock<ISpotifyStatus>();
            spotifyStatusMock.Setup(x => x.CurrentTrack).Returns(spotifyPaused);
            spotifyStatusMock.Setup(x => x.GetTrack()).ReturnsAsync(spotifyPaused);

            var spotifyProcessMock = new Mock<ISpotifyProcess>();
            spotifyProcessMock.Setup(x => x.GetSpotifyStatus()).ReturnsAsync(spotifyStatusMock.Object);

            var spotifyHandler = new SpotifyHandler(spotifyProcessMock.Object)
            {
                ListenForEvents = true
            };

            // initial track
            Assert.Equal(new Track(), spotifyHandler.Track);

            await spotifyHandler.TriggerEvents();

            // updated track
            Assert.Equal(spotifyPaused, spotifyHandler.Track);
            spotifyProcessMock.Verify(n => n.GetSpotifyStatus(), Times.Once);

            spotifyHandler.Dispose();
        }

        [Fact]
        internal async Task TriggerEvents_ReturnsPlayingTrack()
        {
            var paused = new Track {Playing = false};
            var playing = new Track {Playing = false};

            var spotifyStatusMock = new Mock<ISpotifyStatus>();
            spotifyStatusMock.Setup(x => x.CurrentTrack).Returns(playing);
            spotifyStatusMock.Setup(x => x.GetTrack()).ReturnsAsync(playing);

            var spotifyProcessMock = new Mock<ISpotifyProcess>();
            spotifyProcessMock.Setup(x => x.GetSpotifyStatus()).ReturnsAsync(spotifyStatusMock.Object);

            var spotifyHandler = new SpotifyHandler(spotifyProcessMock.Object)
            {
                ListenForEvents = true
            };
            spotifyHandler.Track = paused;

            var eventPlaying = false;
            spotifyHandler.OnPlayStateChange += delegate(object sender, PlayStateEventArgs e)
            {
                eventPlaying = e.Playing;
                Assert.True(eventPlaying);
            };

            await spotifyHandler.TriggerEvents();

            spotifyProcessMock.Verify(n => n.GetSpotifyStatus(), Times.Once);

            spotifyHandler.Dispose();
        }

        [Fact]
        internal async Task TriggerEvents_ReturnsNewestTrack()
        {
            var oldTrack = new Track
            {
                Artist = "Artist",
                Title = "Title",
                TitleExtended = "Remastered",
                Playing = true,
                CurrentPosition = 240,
                Length = 240
            };
            var newestTrack = new Track
            {
                Artist = "Artist",
                Title = "Title",
                TitleExtended = "Live",
                Playing = true,
                Length = 240
            };

            var spotifyStatusMock = new Mock<ISpotifyStatus>();
            spotifyStatusMock.Setup(x => x.CurrentTrack).Returns(newestTrack);
            spotifyStatusMock.Setup(x => x.GetTrack()).ReturnsAsync(newestTrack);

            var spotifyProcessMock = new Mock<ISpotifyProcess>();
            spotifyProcessMock.Setup(x => x.GetSpotifyStatus()).ReturnsAsync(spotifyStatusMock.Object);

            var spotifyHandler = new SpotifyHandler(spotifyProcessMock.Object)
            {
                ListenForEvents = true
            };
            spotifyHandler.Track = oldTrack;

            Track eventNewTrack = null;
            Track eventOldTrack = null;
            spotifyHandler.OnTrackChange += delegate(object sender, TrackChangeEventArgs e)
            {
                eventOldTrack = e.OldTrack;
                eventNewTrack = e.NewTrack;
                Assert.Equal(oldTrack, eventOldTrack);
                Assert.Equal(newestTrack, eventNewTrack);
            };

            int? eventTrackTime = null;
            spotifyHandler.OnTrackTimeChange += delegate(object sender, TrackTimeChangeEventArgs e)
            {
                eventTrackTime = e.TrackTime;
                Assert.Equal(0, eventTrackTime);
            };

            await spotifyHandler.TriggerEvents();

            spotifyProcessMock.Verify(n => n.GetSpotifyStatus(), Times.Once);

            spotifyHandler.Dispose();
        }

        [Fact]
        internal async Task TriggerEvents_ReturnsCurrentTrack()
        {
            var track = new Track
            {
                Artist = "Artist",
                Title = "Title",
                TitleExtended = "Live",
                Playing = true,
                CurrentPosition = 10,
                Length = 230
            };

            var spotifyStatusMock = new Mock<ISpotifyStatus>();
            spotifyStatusMock.Setup(x => x.CurrentTrack).Returns(track);
            spotifyStatusMock.Setup(x => x.GetTrack()).ReturnsAsync(track);

            var spotifyProcessMock = new Mock<ISpotifyProcess>();
            spotifyProcessMock.Setup(x => x.GetSpotifyStatus()).ReturnsAsync(spotifyStatusMock.Object);

            var spotifyHandler = new SpotifyHandler(spotifyProcessMock.Object)
            {
                ListenForEvents = true
            };

            var oldTrackStatus = track;
            oldTrackStatus.CurrentPosition = 9;
            spotifyHandler.Track = oldTrackStatus;

            int? eventTrackTime = null;
            spotifyHandler.OnTrackTimeChange += delegate(object sender, TrackTimeChangeEventArgs e)
            {
                eventTrackTime = e.TrackTime;
                Assert.Equal(track.CurrentPosition, eventTrackTime);
            };

            await spotifyHandler.TriggerEvents();

            spotifyProcessMock.Verify(n => n.GetSpotifyStatus(), Times.Once);

            spotifyHandler.Dispose();
        }
    }
}