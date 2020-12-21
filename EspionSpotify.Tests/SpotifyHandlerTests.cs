using EspionSpotify.Events;
using EspionSpotify.Models;
using EspionSpotify.Spotify;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace EspionSpotify.Tests
{
    public class SpotifyHandlerTests
    {
        [Fact]
        internal void SpotifyHandler_ReturnsSpotifyProcess()
        {
            var spotifyProcessMock = new Moq.Mock<ISpotifyProcess>();

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
            var spotifyProcessMock = new Moq.Mock<ISpotifyProcess>();

            var spotifyHandler = new SpotifyHandler(spotifyProcessMock.Object);
            spotifyHandler.Dispose();

            Assert.False(spotifyHandler.EventTimer.Enabled);
            Assert.False(spotifyHandler.SongTimer.Enabled);
        }

        [Fact]
        internal void TickEventSpotifyIdling_ReturnsNoEvent()
        {
            var spotifyProcessMock = new Moq.Mock<ISpotifyProcess>();

            var spotifyHandler = new SpotifyHandler(spotifyProcessMock.Object)
            {
                ListenForEvents = true
            };
            spotifyHandler.SongTimer.Start();

            var eventPlaying = false;
            spotifyHandler.OnPlayStateChange += delegate (object sender, PlayStateEventArgs e)
            {
                eventPlaying = e.Playing;
                Assert.False(eventPlaying);
            };

            Track eventNewTrack = null;
            Track eventOldTrack = null;
            spotifyHandler.OnTrackChange += delegate (object sender, TrackChangeEventArgs e)
            {
                eventNewTrack = e.NewTrack;
                eventOldTrack = e.OldTrack;
                Assert.Null(eventNewTrack);
                Assert.Null(eventOldTrack);
            };

            int? eventTrackTime = null;
            spotifyHandler.OnTrackTimeChange += delegate (object sender, TrackTimeChangeEventArgs e)
            {
                eventTrackTime = e.TrackTime;
                Assert.Equal(0, eventTrackTime);
            };
        }

        [Fact]
        internal void ElapsedEventTick_ReturnsInitialSpotifyTrack()
        {
            var spotifyPaused = new Track();

            var spotifyStatusMock = new Moq.Mock<ISpotifyStatus>();
            spotifyStatusMock.Setup(x => x.CurrentTrack).Returns(spotifyPaused);
            spotifyStatusMock.Setup(x => x.GetTrack()).Returns(spotifyPaused);

            var spotifyProcessMock = new Moq.Mock<ISpotifyProcess>();
            spotifyProcessMock.Setup(x => x.GetSpotifyStatus()).ReturnsAsync(spotifyStatusMock.Object);

            var spotifyHandler = new SpotifyHandler(spotifyProcessMock.Object)
            {
                ListenForEvents = true
            };

            // initial track
            Assert.Null(spotifyHandler.Track);

            spotifyHandler.ElapsedEventTick(new object { }, new EventArgs() as System.Timers.ElapsedEventArgs);

            // updated track
            Assert.Equal(spotifyPaused, spotifyHandler.Track);
            spotifyProcessMock.Verify(n => n.GetSpotifyStatus(), Times.Once);
        }

        [Fact]
        internal void ElapsedEventTick_ReturnsPlayingTrack()
        {
            var paused = new Track() { Playing = false };
            var playing = new Track() { Playing = false };

            var spotifyStatusMock = new Moq.Mock<ISpotifyStatus>();
            spotifyStatusMock.Setup(x => x.CurrentTrack).Returns(playing);
            spotifyStatusMock.Setup(x => x.GetTrack()).Returns(playing);

            var spotifyProcessMock = new Moq.Mock<ISpotifyProcess>();
            spotifyProcessMock.Setup(x => x.GetSpotifyStatus()).ReturnsAsync(spotifyStatusMock.Object);

            var spotifyHandler = new SpotifyHandler(spotifyProcessMock.Object)
            {
                ListenForEvents = true
            };
            spotifyHandler.Track = paused;

            var eventPlaying = false;
            spotifyHandler.OnPlayStateChange += delegate (object sender, PlayStateEventArgs e)
            {
                eventPlaying = e.Playing;
                Assert.True(eventPlaying);
            };

            spotifyHandler.ElapsedEventTick(new object { }, new EventArgs() as System.Timers.ElapsedEventArgs);

            spotifyProcessMock.Verify(n => n.GetSpotifyStatus(), Times.Once);
        }

        [Fact]
        internal void ElapsedEventTick_ReturnsNewestTrack()
        {
            var oldTrack = new Track()
            {
                Artist = "Artist",
                Title = "Title",
                TitleExtended = "Remastered",
                Playing = true ,
                CurrentPosition = 240,
                Length = 240,
            };
            var newestTrack = new Track()
            {
                Artist = "Artist",
                Title = "Title",
                TitleExtended = "Live",
                Playing = true,
                Length = 240,
            };

            var spotifyStatusMock = new Moq.Mock<ISpotifyStatus>();
            spotifyStatusMock.Setup(x => x.CurrentTrack).Returns(newestTrack);
            spotifyStatusMock.Setup(x => x.GetTrack()).Returns(newestTrack);

            var spotifyProcessMock = new Moq.Mock<ISpotifyProcess>();
            spotifyProcessMock.Setup(x => x.GetSpotifyStatus()).ReturnsAsync(spotifyStatusMock.Object);

            var spotifyHandler = new SpotifyHandler(spotifyProcessMock.Object)
            {
                ListenForEvents = true
            };
            spotifyHandler.Track = oldTrack;

            Track eventNewTrack = null;
            Track eventOldTrack = null;
            spotifyHandler.OnTrackChange += delegate (object sender, TrackChangeEventArgs e)
            {
                eventOldTrack = e.OldTrack;
                eventNewTrack = e.NewTrack;
                Assert.Equal(oldTrack, eventOldTrack);
                Assert.Equal(newestTrack, eventNewTrack);
            };

            int? eventTrackTime = null;
            spotifyHandler.OnTrackTimeChange += delegate (object sender, TrackTimeChangeEventArgs e)
            {
                eventTrackTime = e.TrackTime;
                Assert.Equal(0, eventTrackTime);
            };

            spotifyHandler.ElapsedEventTick(new object { }, new EventArgs() as System.Timers.ElapsedEventArgs);

            spotifyProcessMock.Verify(n => n.GetSpotifyStatus(), Times.Once);
        }

        [Fact]
        internal void ElapsedEventTick_ReturnsCurrentTrack()
        {
            var track = new Track()
            {
                Artist = "Artist",
                Title = "Title",
                TitleExtended = "Live",
                Playing = true,
                CurrentPosition = 10,
                Length = 230,
            };

            var spotifyStatusMock = new Moq.Mock<ISpotifyStatus>();
            spotifyStatusMock.Setup(x => x.CurrentTrack).Returns(track);
            spotifyStatusMock.Setup(x => x.GetTrack()).Returns(track);

            var spotifyProcessMock = new Moq.Mock<ISpotifyProcess>();
            spotifyProcessMock.Setup(x => x.GetSpotifyStatus()).ReturnsAsync(spotifyStatusMock.Object);

            var spotifyHandler = new SpotifyHandler(spotifyProcessMock.Object)
            {
                ListenForEvents = true
            };

            var oldTrackStatus = track;
            oldTrackStatus.CurrentPosition = 9;
            spotifyHandler.Track = oldTrackStatus;

            int? eventTrackTime = null;
            spotifyHandler.OnTrackTimeChange += delegate (object sender, TrackTimeChangeEventArgs e)
            {
                eventTrackTime = e.TrackTime;
                Assert.Equal(track.CurrentPosition, eventTrackTime);
            };

            spotifyHandler.ElapsedEventTick(new object { }, new EventArgs() as System.Timers.ElapsedEventArgs);

            spotifyProcessMock.Verify(n => n.GetSpotifyStatus(), Times.Once);
        }
    }
}
