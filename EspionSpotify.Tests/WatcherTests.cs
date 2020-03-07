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
        private readonly IFileSystem _fileSystem;
        private readonly UserSettings _userSettings;

        public WatcherTests()
        {
            _formMock = new Moq.Mock<IFrmEspionSpotify>().Object;
            _userSettings = new UserSettings();

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\path\Artist_-_Dont_Overwrite_Me.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });
        }

        [Fact]
        internal void RecorderUpAndRunning_ReturnsStatus()
        {
            var watcher = new Watcher(_formMock, _userSettings, new Track(), _fileSystem);

            Assert.False(watcher.RecorderUpAndRunning);
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
            var userSettingsCanDuplicate = new UserSettings { DuplicateAlreadyRecordedTrack = true, OutputPath = @"C:\path", TrackTitleSeparator = "_", MediaFormat = Enums.MediaFormat.Mp3 };
            var watcherTrackFoundCanDuplicate = new Watcher(_formMock, userSettingsCanDuplicate, new Track() { Artist = "Artist", Title = "Dont Overwrite Me" }, _fileSystem);

            Assert.False(watcherTrackFoundCanDuplicate.IsTrackExists);
        }

        [Fact]
        internal void IsTrackExists_TruthyWhenTrackFoundPlaying()
        {
            var userSettings = new UserSettings { OutputPath = @"C:\path", TrackTitleSeparator = "_", MediaFormat = Enums.MediaFormat.Mp3 };
            var watcherTrackFound = new Watcher(_formMock, userSettings, new Track() { Artist = "Artist", Title = "Dont Overwrite Me", Playing = true }, _fileSystem);

            Assert.True(watcherTrackFound.IsTrackExists);
        }

        [Fact]
        internal void IsTypeAllowed_ReturnsIfCurrentTrackTypeCanBeRecorded()
        {
            var watcher = new Watcher(_formMock, _userSettings, new Track(), _fileSystem);
            var track = new Track();
            var isTypeAllowed = track.IsNormal() || (_userSettings.RecordUnknownTrackTypeEnabled && track.Playing);

            Assert.Equal(isTypeAllowed, watcher.IsTypeAllowed);
        }

        [Fact]
        internal void IsOldSong_ReturnsIfCurrentTrackIsOld()
        {
            var watcher = new Watcher(_formMock, _userSettings, new Track(), _fileSystem);
            var track = new Track();
            var isOld = _userSettings.EndingTrackDelayEnabled && track.Length > 0 && track.CurrentPosition > track.Length - 5;

            Assert.Equal(isOld, watcher.IsOldSong);
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
