using EspionSpotify.Events;
using EspionSpotify.Models;
using EspionSpotify.Spotify;
using Moq;
using System.Threading.Tasks;
using System.Timers;
using Xunit;

namespace EspionSpotify.Tests
{
    public class SpotifyHandlerTests
    {
        [Fact]
        private void SpotifyHandler_ReturnsSpotifyProcess()
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
        private void Dispose_ReturnsTimerOff()
        {
            var spotifyProcessMock = new Mock<ISpotifyProcess>();

            var spotifyHandler = new SpotifyHandler(spotifyProcessMock.Object);
            spotifyHandler.Dispose();

            Assert.False(spotifyHandler.EventTimer.Enabled);
            Assert.False(spotifyHandler.SongTimer.Enabled);
        }

        [Fact]
        private async void TickEventSpotifyIdling_ReturnsNoEvent()
        {
            var track = new Track();

            var spotifyStatusMock = new Mock<ISpotifyStatus>();
            spotifyStatusMock.Setup(x => x.Track).Returns(track);
            spotifyStatusMock.Setup(x => x.GetTrack()).Returns(track);

            var spotifyProcessMock = new Mock<ISpotifyProcess>();
            spotifyProcessMock.Setup(x => x.GetSpotifyStatus()).Returns(spotifyStatusMock.Object);

            var spotifyHandler = new SpotifyHandler(spotifyProcessMock.Object)
            {
                ListenForEvents = true
            };
            spotifyHandler.SongTimer.Start();

            var eventPlaying = false;
            spotifyHandler.OnPlayStateChange += delegate (object sender, PlayStateEventArgs e)
            {
                eventPlaying = e.Playing;
            };

            Track eventNewTrack = null;
            Track eventOldTrack = null;
            spotifyHandler.OnTrackChange += delegate (object sender, TrackChangeEventArgs e)
            {
                eventNewTrack = e.NewTrack;
                eventOldTrack = e.OldTrack;
            };

            int? eventTrackTime = null;
            spotifyHandler.OnTrackTimeChange += delegate (object sender, TrackTimeChangeEventArgs e)
            {
                eventTrackTime = e.TrackTime;
            };

            // initial track
            Assert.Null(spotifyHandler.Track);

            await Task.Delay(500);

            // updated track
            Assert.Equal(track, spotifyHandler.Track);

            // events
            Assert.False(eventPlaying);
            Assert.Null(eventNewTrack);
            Assert.Null(eventOldTrack);
            Assert.Equal(0, eventTrackTime);
        }

        [Fact]
        private async void NewTrack_ReturnsAllEvents()
        {
            var previousTrack = new Track();
            var currentTrack = new Track
            {
                Title = "Song Title",
                Artist = "Artist Name",
                Ad = false,
                Playing = true,
                TitleExtended = "Live"
            };

            var spotifyStatusMock = new Mock<ISpotifyStatus>();
            spotifyStatusMock.Setup(x => x.Track).Returns(currentTrack);
            spotifyStatusMock.Setup(x => x.GetTrack()).Returns(currentTrack);

            var spotifyProcessMock = new Mock<ISpotifyProcess>();
            spotifyProcessMock.Setup(x => x.GetSpotifyStatus()).Returns(spotifyStatusMock.Object);

            var spotifyHandler = new SpotifyHandler(spotifyProcessMock.Object)
            {
                ListenForEvents = true,
                Track = previousTrack
            };

            var eventPlaying = false;
            spotifyHandler.OnPlayStateChange += delegate (object sender, PlayStateEventArgs e)
            {
                eventPlaying = e.Playing;
            };

            Track eventNewTrack = null;
            Track eventOldTrack = null;
            spotifyHandler.OnTrackChange += delegate (object sender, TrackChangeEventArgs e)
            {
                eventNewTrack = e.NewTrack;
                eventOldTrack = e.OldTrack;
            };

            int? eventTrackTime = null;
            spotifyHandler.OnTrackTimeChange += delegate (object sender, TrackTimeChangeEventArgs e)
            {
                eventTrackTime = e.TrackTime;
            };

            // initial track
            Assert.Equal(previousTrack, spotifyHandler.Track);

            await Task.Delay(500);

            // updated track
            Assert.Equal(currentTrack, spotifyHandler.Track);

            // events
            Assert.True(eventPlaying);
            Assert.Equal(currentTrack, eventNewTrack);
            Assert.Equal(previousTrack, eventOldTrack);
            Assert.Equal(0, eventTrackTime);
        }

        [Fact]
        private async void TickEventSameTrackPlaying_ReturnsTrackTimeEvent()
        {
            var track = new Track
            {
                Title = "Song Title",
                Artist = "Artist Name",
                Ad = false,
                Playing = true,
                TitleExtended = "Live"
            };

            var spotifyStatusMock = new Mock<ISpotifyStatus>();
            spotifyStatusMock.Setup(x => x.Track).Returns(track);
            spotifyStatusMock.Setup(x => x.GetTrack()).Returns(track);

            var spotifyProcessMock = new Mock<ISpotifyProcess>();
            spotifyProcessMock.Setup(x => x.GetSpotifyStatus()).Returns(spotifyStatusMock.Object);

            var spotifyHandler = new SpotifyHandler(spotifyProcessMock.Object)
            {
                ListenForEvents = true,
                Track = track
            };
            spotifyHandler.SongTimer.Start();

            var eventPlaying = false;
            spotifyHandler.OnPlayStateChange += delegate (object sender, PlayStateEventArgs e)
            {
                eventPlaying = e.Playing;
            };

            Track eventNewTrack = null;
            Track eventOldTrack = null;
            spotifyHandler.OnTrackChange += delegate (object sender, TrackChangeEventArgs e)
            {
                eventNewTrack = e.NewTrack;
                eventOldTrack = e.OldTrack;
            };

            int? eventTrackTime = null;
            spotifyHandler.OnTrackTimeChange += delegate(object sender, TrackTimeChangeEventArgs e)
            {
                eventTrackTime = e.TrackTime;
            };

            // initial track
            Assert.Equal(track, spotifyHandler.Track);

            await Task.Delay(1500);

            // updated track
            Assert.Equal(track, spotifyHandler.Track);

            // events
            Assert.False(eventPlaying);
            Assert.Null(eventNewTrack);
            Assert.Null(eventOldTrack);
            Assert.Equal(1, eventTrackTime);
        }
    }
}
