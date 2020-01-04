using EspionSpotify.Models;
using NAudio.Lame;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.RegularExpressions;
using Xunit;

namespace EspionSpotify.Tests
{
    public class FileManagerTests
    {
        private readonly UserSettings _userSettings;
        private readonly Track _track;
        private readonly FileManager _fileManager;
        private readonly IFileSystem _fileSystem;
        private readonly string _path = @"C:\path";

        private readonly string _windowsExlcudedChars = $"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()))}]";

        public FileManagerTests()
        {
            _userSettings = new UserSettings
            {
                OutputPath = _path,
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

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\path\Empty Artist", new MockDirectoryData() },
                { @"C:\path\Artist\Delete Me.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem);
        }

        [Fact]
        internal void GetOutputFile_ReturnsFileNames()
        {
            var outputFile = _fileManager.GetOutputFile(_path);

            Assert.Equal(_path, outputFile.Path);
            Assert.Equal(_track.ToString(), outputFile.File);
            Assert.Equal(_userSettings.MediaFormat.ToString().ToLower(), outputFile.Extension);
            Assert.Equal(_userSettings.TrackTitleSeparator, outputFile.Separator);

            Assert.Equal(@"C:\path\Artist - Title - Live.spytify", outputFile.ToPendingFileString());
            Assert.Equal(@"C:\path\Artist - Title - Live.mp3.spytify", outputFile.ToTranscodingToMP3String());
            Assert.Equal(@"C:\path\Artist - Title - Live.mp3", outputFile.ToString());
        }

        [Fact]
        internal void BuildFileName_ReturnsFileName()
        {
            var fileName = _fileManager.GetOutputFile(_path).ToString();

            Assert.Equal(@"C:\path\Artist - Title - Live.mp3", fileName);
        }

        [Fact]
        internal void BuildFileName_ReturnsFileNameOrderNumbered()
        {
            _userSettings.OrderNumberInfrontOfFileEnabled = true;
            _userSettings.TrackTitleSeparator = "_";
            _userSettings.InternalOrderNumber = 100;

            var fileName = _fileManager.GetOutputFile(_path).ToString();

            Assert.Equal(@"C:\path\100_Artist_-_Title_-_Live.mp3", fileName);
        }

        [Theory]
        [InlineData(true, @"C:\path\Artist\Title - Live.mp3")]
        [InlineData(false, @"C:\path\Artist - Title - Live.mp3")]
        internal void BuildFileName_ReturnsFileNameGroupByFolders(bool isGroupByArtistFolder, string expectedResult)
        {
            _userSettings.GroupByFoldersEnabled = isGroupByArtistFolder;

            var fileName = _fileManager.GetOutputFile(_path).ToString();

            Assert.Equal(expectedResult, fileName);
        }

        [Fact]
        internal void BuildFileName_ReturnsFileNameWav()
        {
            _userSettings.MediaFormat = Enums.MediaFormat.Wav;

            var fileName = _fileManager.GetOutputFile(_path).ToString();

            Assert.Equal(@"C:\path\Artist - Title - Live.wav", fileName);
        }

        [Fact]
        internal void GetFolderPath_ReturnsNoArtistFolderPath()
        {
            var artistFolder = FileManager.GetFolderPath(_track, _userSettings, _fileSystem);

            Assert.Null(artistFolder);
        }

        [Fact]
        internal void GetFolderPath_ReturnsArtistFolderPath()
        {
            _userSettings.GroupByFoldersEnabled = true;

            var artistDir = Regex.Replace(_track.Artist, _windowsExlcudedChars, string.Empty);

            var artistFolder = FileManager.GetFolderPath(_track, _userSettings, _fileSystem);

            Assert.Equal($@"\{artistDir}", artistFolder);
        }

        [Fact]
        internal void IsPathFileNameExists_ReturnsExists()
        {
            var result = FileManager.IsPathFileNameExists(_track, _userSettings, _fileSystem);
            Assert.False(result);
        }

        [Fact]
        internal void DeleteFile_DeletesFile()
        {
            _track.Title = "Delete Me";
            _track.TitleExtended = "";

            var extension = _userSettings.MediaFormat.ToString().ToLower();
            var currentFile = $@"{_userSettings.OutputPath}\{_track.ToString()}.{extension}";

            _fileManager.DeleteFile(currentFile);

            Assert.False(_fileSystem.File.Exists(currentFile));
        }

        [Fact]
        internal void DeleteFile_DeletesFolderWhenGroupByFoldersEnabled()
        {
            _userSettings.GroupByFoldersEnabled = true;
            _track.Artist = "Empty Artist";

            var artistDir = Regex.Replace(_track.Artist, _windowsExlcudedChars, string.Empty);
            var extension = _userSettings.MediaFormat.ToString().ToLower();
            var currentFile = $@"{_userSettings.OutputPath}\{artistDir}\{_track.ToTitleString()}.{extension}";

            _fileManager.DeleteFile(currentFile);

            Assert.False(_fileSystem.Directory.Exists($@"{_userSettings.OutputPath}\{artistDir}"));
            Assert.False(_fileSystem.File.Exists(currentFile));
        }
    }
}
