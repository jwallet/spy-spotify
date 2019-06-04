using EspionSpotify.Models;
using Xunit;

namespace EspionSpotify.Tests
{
    public class WatcherTests
    {
        private readonly IFrmEspionSpotify _formMock;
        private UserSettings _userSettings;

        public WatcherTests()
        {
            _formMock = new Moq.Mock<IFrmEspionSpotify>().Object;
            _userSettings = new UserSettings();
        }

        [Fact]
        private void RecorderUpAndRunning_ReturnsStatus()
        {
            var watcher = new Watcher(_formMock, _userSettings);

            Assert.False(watcher.RecorderUpAndRunning);
        }

        [Fact]
        private void NumTrackActivated_ReturnsIfOrderNumberIsSet()
        {
            var userSettings = new UserSettings();
            var watcherFalsy = new Watcher(_formMock, userSettings);
            Assert.False(watcherFalsy.NumTrackActivated);

            userSettings.OrderNumberInfrontOfFileEnabled = true;
            userSettings.InternalOrderNumber = 1;
            var watcherTruthy = new Watcher(_formMock, userSettings);
            Assert.True(watcherTruthy.NumTrackActivated);

            userSettings.OrderNumberInfrontOfFileEnabled = false;
            userSettings.OrderNumberInMediaTagEnabled = true;
            userSettings.InternalOrderNumber = 1;
            var watcherTruthyTwo = new Watcher(_formMock, userSettings);
            Assert.True(watcherTruthyTwo.NumTrackActivated);
        }

        [Fact]
        private void AdPlaying_ReturnsCurrentTrackAdStatus()
        {
            var watcher = new Watcher(_formMock, _userSettings);

            Assert.False(watcher.AdPlaying);
        }

        [Fact]
        private void SongTitle_ReturnsCurrentTrack()
        {
            var watcher = new Watcher(_formMock, _userSettings);

            Assert.Equal(new Track().ToString(), watcher.SongTitle);
        }

        [Fact]
        private void IsTypeAllowed_ReturnsIfCurrentTrackTypeCanBeRecorded()
        {
            var watcher = new Watcher(_formMock, _userSettings);
            var track = new Track();
            var isTypeAllowed = track.IsNormal() || (_userSettings.RecordUnknownTrackTypeEnabled && track.Playing);

            Assert.Equal(isTypeAllowed, watcher.IsTypeAllowed);
        }

        [Fact]
        private void IsOldSong_ReturnsIfCurrentTrackIsOld()
        {
            var watcher = new Watcher(_formMock, _userSettings);
            var track = new Track();
            var isOld = _userSettings.EndingTrackDelayEnabled && track.Length > 0 && track.CurrentPosition > track.Length - 5;

            Assert.Equal(isOld, watcher.IsOldSong);
        }

        [Fact]
        private void IsNewTrack_ReturnsExpectedResults()
        {
            var watcher = new Watcher(_formMock, _userSettings);

            Assert.False(watcher.IsNewTrack(null));
            Assert.False(watcher.IsNewTrack(new Track()));
            Assert.True(watcher.IsNewTrack(new Track { Artist = "Artist", Title = "Title" }));
        }
    }
}
