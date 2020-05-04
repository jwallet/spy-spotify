using EspionSpotify.Models;
using Xunit;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Collections.Generic;

namespace EspionSpotify.Tests
{
    public class WatcherTests
    {
        private readonly IFrmEspionSpotify _formMock;
        private IFileSystem _fileSystem;
        private readonly UserSettings _userSettings;

        public WatcherTests()
        {
            _formMock = new Moq.Mock<IFrmEspionSpotify>().Object;
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
            var watcher = new Watcher(_formMock, _userSettings, new Track(), _fileSystem);

            Assert.False(watcher.RecorderUpAndRunning);
        }

        [Fact]
        internal void RecorderUpAndRunning_FalsyWhenRecorderNotRunning()
        {
            var recorder = new Recorder();
            var watcher = new Watcher(_formMock, _userSettings, new Track(), _fileSystem, recorder);

            Assert.False(watcher.RecorderUpAndRunning);
        }

        [Fact]
        internal void RecorderUpAndRunning_TruthyWhenRecorderRunning()
        {
            var recorder = new Recorder() { Running = true };
            var watcher = new Watcher(_formMock, _userSettings, new Track(), _fileSystem, recorder);

            Assert.True(watcher.RecorderUpAndRunning);
        }

        [Fact]
        internal void IsRecordUnknownActive_FalsyWhenSpotifyInactive()
        {
            var userSettings = new UserSettings { RecordUnknownTrackTypeEnabled = true };
            var watcher = new Watcher(_formMock, userSettings, new Track() { Artist = "Spotify Free" }, _fileSystem);

            Assert.False(watcher.IsRecordUnknownActive);
        }

        [Fact]
        internal void IsRecordUnknownActive_FalsyWhenSpotifyAdPlaying()
        {
            var userSettings = new UserSettings { RecordUnknownTrackTypeEnabled = true };
            var watcher = new Watcher(_formMock, userSettings, new Track() { Playing = true, Artist = "Spotify Free" }, _fileSystem);

            Assert.False(watcher.IsRecordUnknownActive);
        }

        [Fact]
        internal void IsRecordUnknownActive_FalsyWhenDisabledAndAnyTitlePlaying()
        {
            var userSettings = new UserSettings { RecordUnknownTrackTypeEnabled = false };
            var watcher = new Watcher(_formMock, userSettings, new Track { Playing = true, Artist = "Podcast" }, _fileSystem);

            Assert.False(watcher.IsRecordUnknownActive);
        }

        [Fact]
        internal void IsRecordUnknownActive_FalsyWhenAnyTitlePlaying()
        {
            var userSettings = new UserSettings { RecordUnknownTrackTypeEnabled = true };
            var watcher = new Watcher(_formMock, userSettings, new Track { Playing = true, Artist = "Podcast", Ad = false }, _fileSystem);

            Assert.False(watcher.IsRecordUnknownActive);
        }

        [Fact]
        internal void IsRecordUnknownActive_TruthyWhenAnyTitlePlayingAsAd()
        {
            var userSettings = new UserSettings { RecordUnknownTrackTypeEnabled = true };
            var watcher = new Watcher(_formMock, userSettings, new Track { Playing = true, Artist = "Podcast", Ad = true }, _fileSystem);

            Assert.True(watcher.IsRecordUnknownActive);
        }

        [Fact]
        internal void IsRecordUnknownActive_TruthyWhenTrackIsPlaying()
        {
            var userSettings = new UserSettings { RecordUnknownTrackTypeEnabled = true };
            var watcher = new Watcher(_formMock, userSettings, new Track { Playing = true, Artist = "#3 - Podcast", Ad = false }, _fileSystem);

            Assert.False(watcher.IsRecordUnknownActive);
        }

        [Fact]
        internal void IsTrackExists_FalsyWhenTrackNotFound()
        {
            var userSettings = new UserSettings { OutputPath = @"C:\path", TrackTitleSeparator = "_", MediaFormat = Enums.MediaFormat.Mp3 };
            var watcherTrackNotFound = new Watcher(_formMock, userSettings, new Track() { Artist = "Artist", Title = "Title" }, _fileSystem);

            Assert.False(watcherTrackNotFound.IsTrackExists);
        }

        [Fact]
        internal void IsTrackExists_FalsyWhenCanDuplicateTrackFound()
        {
            var userSettingsCanDuplicate = new UserSettings {
                RecordRecordingsStatus = Enums.RecordRecordingsStatus.Duplicate,
                OutputPath = @"C:\path",
                TrackTitleSeparator = "_",
                MediaFormat = Enums.MediaFormat.Mp3
            };
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\path\Artist_-_Dont_Overwrite_Me.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });
            var watcherTrackFoundCanDuplicate = new Watcher(
                _formMock, userSettingsCanDuplicate,
                new Track() { Artist = "Artist", Title = "Dont Overwrite Me" },
                _fileSystem);

            Assert.False(watcherTrackFoundCanDuplicate.IsTrackExists);
        }

        [Fact]
        internal void IsSkipTrackActive_FalsyWhenTrackFoundButOverwriteEnabled()
        {
            var userSettingsCanDuplicate = new UserSettings
            {
                RecordRecordingsStatus = Enums.RecordRecordingsStatus.Overwrite,
                OutputPath = @"C:\path",
                TrackTitleSeparator = "_",
                MediaFormat = Enums.MediaFormat.Mp3
            };
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\path\Artist_-_Dont_Overwrite_Me.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });
            var watcherTrackFoundCanDuplicate = new Watcher(
                _formMock, userSettingsCanDuplicate,
                new Track() { Artist = "Artist", Title = "Dont Overwrite Me" },
                _fileSystem);

            Assert.False(watcherTrackFoundCanDuplicate.IsSkipTrackActive);
        }

        [Fact]
        internal void IsTrackExists_TruthyWhenTrackFoundPlaying()
        {
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\path\Artist_-_Existing_Track.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });
            var userSettings = new UserSettings { OutputPath = @"C:\path", TrackTitleSeparator = "_", MediaFormat = Enums.MediaFormat.Mp3 };
            var watcherTrackFound = new Watcher(_formMock, userSettings, new Track() { Artist = "Artist", Title = "Existing Track", Playing = true }, _fileSystem);

            Assert.True(watcherTrackFound.IsTrackExists);
        }

        [Theory]
        [InlineData(false, false, true)]
        [InlineData(true, false, true)]
        [InlineData(false, true, false)]
        [InlineData(true, true, false)]
        internal void IsTypeAllowed_ReturnsExpectedResults(bool recordUnknownTrackTypeEnabled, bool isSpotify, bool expected)
        {
            // Track IsNormal getter is tested in Track tests
            // Is titled Spotify tested in Spotify Status tests
            var track = new Track() { Artist = "A", Title = "B", Ad = false, Playing = true };
            if (isSpotify)
            {
                track.Playing = false;
                track.Artist = "Spotify";
                track.Title = null;
            }
            _userSettings.RecordUnknownTrackTypeEnabled = recordUnknownTrackTypeEnabled;
            var watcher = new Watcher(_formMock, _userSettings, track, _fileSystem);

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
        internal void IsOldSong_ReturnsExpectedResults(bool endingTrackDelayEnabled, int trackCurrentPosition, int trackLength, bool expected)
        {
            _userSettings.EndingTrackDelayEnabled = endingTrackDelayEnabled;
            var track = new Track() { CurrentPosition = trackCurrentPosition, Length = trackLength };
            var watcher = new Watcher(_formMock, _userSettings, track, _fileSystem);

            Assert.Equal(expected, watcher.IsOldSong);
        }

        [Fact]
        internal void IsNewTrack_ReturnsExpectedResults()
        {
            var watcher = new Watcher(_formMock, _userSettings, new Track { Artist = "Spotify Free" }, _fileSystem);

            Assert.False(watcher.IsNewTrack(null));
            Assert.False(watcher.IsNewTrack(new Track()));
            Assert.False(watcher.IsNewTrack(new Track { Artist = "Spotify Free" }));
            Assert.True(watcher.IsNewTrack(new Track { Artist = "Artist", Title = "Title" }));
        }
    }
}
