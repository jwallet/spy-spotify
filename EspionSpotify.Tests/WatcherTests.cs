using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using EspionSpotify.AudioSessions;
using EspionSpotify.Models;
using Moq;
using Xunit;

namespace EspionSpotify.Tests
{
    public class WatcherTests
    {
        private readonly IMainAudioSession _audioSession;
        private readonly IFrmEspionSpotify _formMock;
        private readonly UserSettings _userSettings;
        private readonly IFileSystem _fileSystem;

        public WatcherTests()
        {
            _formMock = new Mock<IFrmEspionSpotify>().Object;
            _audioSession = new Mock<IMainAudioSession>().Object;
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
            _userSettings = new UserSettings();
        }

        [Fact]
        internal void Watcher_ReturnsReadyByDefault()
        {
            Assert.True(Watcher.Ready);
        }

        [Fact]
        internal void RecorderUpAndRunning_FalsyWhenNoRecorder()
        {
            var watcher = new Watcher(
                _formMock,
                _audioSession,
                _userSettings,
                new Track(),
                _fileSystem);

            Assert.False(watcher.RecorderUpAndRunning);
        }

        [Fact]
        internal void RecorderUpAndRunning_FalsyWhenRecorderNotRunning()
        {
            var recorder = new Recorder();
            var watcher = new Watcher(
                _formMock,
                _audioSession,
                _userSettings,
                new Track(),
                _fileSystem,
                recorder);

            Assert.False(watcher.RecorderUpAndRunning);
        }

        [Fact]
        internal void RecorderUpAndRunning_TruthyWhenRecorderRunning()
        {
            var recorder = new Recorder {Running = true};
            var watcher = new Watcher(
                _formMock,
                _audioSession,
                _userSettings,
                new Track(),
                _fileSystem,
                recorder);

            Assert.True(watcher.RecorderUpAndRunning);
        }

        [Theory]
        [InlineData(Constants.SPOTIFY)]
        [InlineData(Constants.SPOTIFYFREE)]
        [InlineData(Constants.SPOTIFYPREMIUM)]
        internal void IsRecordUnknownActive_FalsyWhenSpotifyInactive(string title)
        {
            var userSettings = new UserSettings {RecordEverythingEnabled = true};
            var watcher = new Watcher(
                _formMock,
                _audioSession,
                userSettings,
                new Track {Playing = false, Artist = title},
                _fileSystem);

            Assert.False(watcher.IsRecordUnknownActive);
        }

        [Theory]
        [InlineData(Constants.SPOTIFY)]
        [InlineData(Constants.SPOTIFYFREE)]
        [InlineData(Constants.SPOTIFYPREMIUM)]
        [InlineData(Constants.ADVERTISEMENT)]
        internal void IsRecordUnknownActive_FalsyWhenSpotifyAdPlaying(string title)
        {
            var userSettings = new UserSettings {RecordEverythingEnabled = true};
            var watcher = new Watcher(
                _formMock,
                _audioSession,
                userSettings,
                new Track {Playing = true, Artist = title},
                _fileSystem);

            Assert.False(watcher.IsRecordUnknownActive);
        }

        [Fact]
        internal void IsRecordUnknownActive_FalsyWhenDisabledAndAnyTitlePlaying()
        {
            var userSettings = new UserSettings {RecordEverythingEnabled = false};
            var watcher = new Watcher(
                _formMock,
                _audioSession,
                userSettings,
                new Track {Playing = true, Artist = "Podcast"},
                _fileSystem);

            Assert.False(watcher.IsRecordUnknownActive);
        }

        [Fact]
        internal void IsRecordUnknownActive_ThruthyWhenAnyTitlePlaying()
        {
            var userSettings = new UserSettings {RecordEverythingEnabled = true};
            var watcher = new Watcher(
                _formMock,
                _audioSession,
                userSettings,
                new Track {Playing = true, Artist = "Podcast", Ad = false},
                _fileSystem);

            Assert.True(watcher.IsRecordUnknownActive);
        }

        [Fact]
        internal void IsRecordUnknownActive_TruthyWhenAnyTitlePlayingAsAd()
        {
            var userSettings = new UserSettings {RecordEverythingEnabled = true};
            var watcher = new Watcher(
                _formMock,
                _audioSession,
                userSettings,
                new Track {Playing = true, Artist = "Podcast", Ad = true},
                _fileSystem);

            Assert.True(watcher.IsRecordUnknownActive);
        }

        [Fact]
        internal void IsRecordUnknownActive_FalsyWhenNormalTrackIsPlaying()
        {
            var userSettings = new UserSettings {RecordEverythingEnabled = true};
            var watcher = new Watcher(
                _formMock,
                _audioSession,
                userSettings,
                new Track {Playing = true, Artist = "#3", Title = "Podcast"},
                _fileSystem);

            Assert.False(watcher.IsRecordUnknownActive);
        }

        [Theory]
        [InlineData(false, false, true)]
        [InlineData(true, false, true)]
        [InlineData(false, true, false)]
        [InlineData(true, true, false)]
        internal void IsTypeAllowed_ReturnsExpectedResults(bool recordUnknownTrackTypeEnabled, bool isSpotify,
            bool expected)
        {
            // Track IsNormal getter is tested in Track tests
            // Is titled Spotify tested in Spotify Status tests
            var track = new Track {Artist = "A", Title = "B", Ad = false, Playing = true};
            if (isSpotify)
            {
                track.Playing = false;
                track.Artist = Constants.SPOTIFY;
                track.Title = null;
            }

            _userSettings.RecordEverythingEnabled = recordUnknownTrackTypeEnabled;
            var watcher = new Watcher(_formMock, _audioSession, _userSettings, track, _fileSystem);

            Assert.Equal(expected, watcher.IsTypeAllowed);
        }

        [Theory]
        [InlineData(true, 1, 1, true)]
        [InlineData(true, 70, 60, true)]
        [InlineData(true, 60, 60, true)]
        [InlineData(true, 56, 60, true)]
        [InlineData(true, 1, 60, false)]
        [InlineData(false, 1, 60, false)]
        [InlineData(true, 1, 0, false)]
        [InlineData(true, 0, 5, false)]
        internal void IsOldSong_ReturnsExpectedResults(bool endingTrackDelayEnabled, int trackCurrentPosition,
            int trackLength, bool expected)
        {
            _userSettings.EndingTrackDelayEnabled = endingTrackDelayEnabled;
            var track = new Track {CurrentPosition = trackCurrentPosition, Length = trackLength};
            var watcher = new Watcher(_formMock, _audioSession, _userSettings, track, _fileSystem);

            Assert.Equal(expected, watcher.IsOldSong);
        }

        [Fact]
        internal void IsNewTrack_ReturnsExpectedResults()
        {
            var watcher = new Watcher(
                _formMock,
                _audioSession,
                _userSettings,
                new Track {Artist = Constants.SPOTIFYFREE},
                _fileSystem);

            Assert.False(watcher.IsNewTrack(null));
            Assert.False(watcher.IsNewTrack(new Track()));
            Assert.False(watcher.IsNewTrack(new Track {Artist = Constants.SPOTIFYFREE}));
            Assert.True(watcher.IsNewTrack(new Track {Artist = "Artist", Title = "Title"}));
        }

        [Theory]
        [InlineData(true, 10000, true)]
        [InlineData(true, 9999, true)]
        [InlineData(false, 9999, false)]
        [InlineData(true, 9998, false)]
        internal void IsMaxOrderNumberAsFileExceeded_ReturnsExpectedResults(bool enabled, int orderNumber,
            bool expected)
        {
            _userSettings.OrderNumberInfrontOfFileEnabled = enabled;
            _userSettings.OrderNumberInMediaTagEnabled = true;
            _userSettings.OrderNumberMask = "0000";
            _userSettings.InternalOrderNumber = orderNumber;
            var watcher = new Watcher(_formMock, _audioSession, _userSettings, new Track(), _fileSystem);

            Assert.Equal(expected, watcher.IsMaxOrderNumberAsFileExceeded);
        }
    }
}