using EspionSpotify.Models;
using NAudio.Lame;
using System.IO;
using System.Text.RegularExpressions;
using Xunit;

namespace EspionSpotify.Tests
{
    public class FileManagerTests
    {
        private UserSettings _userSettings;
        private Track _track;
        private FileManager _fileManager;

        private readonly string _windowsExlcudedChars = $"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()))}]";

        public FileManagerTests()
        {
            _userSettings = new UserSettings
            {
                OutputPath = "C:\\path",
                Bitrate = LAMEPreset.ABR_128,
                MediaFormat = Enums.MediaFormat.Mp3,
                MinimumRecordedLengthSeconds = 30,
                GroupByFoldersEnabled = false,
                TrackTitleSeparator = " ",
                OrderNumberInMediaTagEnabled = false,
                OrderNumberInfrontOfFileEnabled = false,
                EndingTrackDelayEnabled = true,
                MuteAdsEnabled = true,
                RecordUnknownTrackTypeEnabled = false,
                InternalOrderNumber = 1,
                DuplicateAlreadyRecordedTrack = true
            };

            _track = new Track
            {
                Title = "Title",
                Artist = "Artist",
                TitleExtended = "Live",
                Ad = false
            };

            _fileManager = new FileManager(_userSettings, _track);
        }

        [Theory]
        [InlineData("Artist - Title", 1, null, "Artist - Title.mp3")]
        [InlineData("002 Artist - Title", 1, null, "002 Artist - Title.mp3")]
        [InlineData("001_Artist_-_Title", 1, null, "001_Artist_-_Title.mp3")]
        [InlineData("Artist - Title", 2, null, "Artist - Title 2.mp3")]
        [InlineData("Artist_-_Title", 4, "C:\\path", "C:\\path\\Artist_-_Title 4.mp3")]
        [InlineData("001 Title", 1, "C:\\path\\Artist", "C:\\path\\Artist\\001 Title.mp3")]
        [InlineData("Artist - Title", 1, "C:\\path", "C:\\path\\Artist - Title.mp3")]
        private void GetFileName_ReturnsTrackNameWithMediaFormat(string songName, int count, string path, string expectedResult)
        {
            var fileName = _fileManager.GetFileName(songName, count, path);

            Assert.Equal(expectedResult, fileName);
        }

        [Theory]
        [InlineData("C:\\path\\Artist", false, "Artist - Title - Live.mp3")]
        [InlineData("C:\\path", false, "Artist - Title - Live.mp3")]
        [InlineData("C:\\path\\Artist", true, "C:\\path\\Artist\\Artist - Title - Live.mp3")]
        [InlineData("C:\\path", true, "C:\\path\\Artist - Title - Live.mp3")]
        private void BuildFileName_ReturnsFileName(string path, bool includePath, string expectedResult)
        {
            var fileName = _fileManager.BuildFileName(path, includePath);

            Assert.Equal(expectedResult, fileName);
        }

        [Theory]
        [InlineData("C:\\path\\Artist", false, "100_Artist_-_Title_-_Live.mp3")]
        [InlineData("C:\\path", false, "100_Artist_-_Title_-_Live.mp3")]
        [InlineData("C:\\path\\Artist", true, "C:\\path\\Artist\\100_Artist_-_Title_-_Live.mp3")]
        [InlineData("C:\\path", true, "C:\\path\\100_Artist_-_Title_-_Live.mp3")]
        private void BuildFileName_ReturnsFileNameOrderNumbered(string path, bool includePath, string expectedResult)
        {
            _userSettings.OrderNumberInfrontOfFileEnabled = true;
            _userSettings.TrackTitleSeparator = "_";
            _userSettings.InternalOrderNumber = 100;

            var fileName = _fileManager.BuildFileName(path, includePath);

            Assert.Equal(expectedResult, fileName);
        }

        [Theory]
        [InlineData("C:\\path\\Artist", false, "Title.mp3")]
        [InlineData("C:\\path", false, "Title.mp3")]
        [InlineData("C:\\path\\Artist", true, "C:\\path\\Artist\\Title.mp3")]
        [InlineData("C:\\path", true, "C:\\path\\Title.mp3")]
        private void BuildFileName_ReturnsFileNameGroupByFolders(string path, bool includePath, string expectedResult)
        {
            _userSettings.GroupByFoldersEnabled = true;

            var fileName = _fileManager.BuildFileName(path, includePath);

            Assert.Equal(expectedResult, fileName);
        }

        [Fact]
        private void BuildFileName_ReturnsFileNameWav()
        {
            _userSettings.MediaFormat = Enums.MediaFormat.Wav;

            var fileName = _fileManager.BuildFileName("C:\\path", true);

            Assert.Equal("C:\\path\\Artist - Title - Live.wav", fileName);
        }

        [Fact]
        private void CreateDirectory_ReturnsNoArtistFolderPath()
        {
            var artistFolder = _fileManager.CreateDirectory();

            Assert.Null(artistFolder);
        }

        [Fact]
        private void CreateDirectory_ReturnsArtistFolderPath()
        {
            _userSettings.GroupByFoldersEnabled = true;

            var artistDir = Regex.Replace(_track.Artist, _windowsExlcudedChars, string.Empty);

            var artistFolder = _fileManager.CreateDirectory();

            Assert.Equal($"//{artistDir}", artistFolder);
        }

        [Fact]
        private void DeleteFile_DeletesFolderWhenGroupByFoldersEnabled()
        {
            var artistDir = Regex.Replace(_track.Artist, _windowsExlcudedChars, string.Empty);
            _userSettings.GroupByFoldersEnabled = true;

            var currentFile = $"{_userSettings.OutputPath}//{artistDir}//{_track.ToString()}";

            _fileManager.DeleteFile(currentFile);

            Assert.False(Directory.Exists($"{_userSettings.OutputPath}//{artistDir}"));
            Assert.False(File.Exists(currentFile));
        }
    }
}
