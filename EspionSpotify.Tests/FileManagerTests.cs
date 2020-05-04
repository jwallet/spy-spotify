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
        private FileManager _fileManager;
        private IFileSystem _fileSystem;
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
                InternalOrderNumber = 1
            };

            _track = new Track
            {
                Title = "Title",
                Artist = "Artist",
                TitleExtended = "Live",
                Album = "Single",
                Ad = false
            };

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());

            _fileManager = new FileManager(_userSettings, _track, _fileSystem);
        }

        [Fact]
        internal void GetOutputFile_ReturnsFileName()
        {
            var outputFile = _fileManager.GetOutputFile();

            Assert.Equal(_path, outputFile.Path);
            Assert.Equal(_track.ToString(), outputFile.File);
            Assert.Equal(_userSettings.MediaFormat.ToString().ToLower(), outputFile.Extension);
            Assert.Equal(_userSettings.TrackTitleSeparator, outputFile.Separator);

            Assert.Equal(@"C:\path\Artist - Title - Live.spytify", outputFile.ToPendingFileString());
            Assert.Equal(@"C:\path\Artist - Title - Live.mp3", outputFile.ToString());
        }

        [Fact]
        internal void GetOutputFile_ThrowsExceptionOnSkipExistingFile()
        {
            _userSettings.RecordRecordingsStatus = Enums.RecordRecordingsStatus.Skip;
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\path\Artist - Title - Live.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem);

            Assert.Throws<InvalidOperationException>(() => _fileManager.GetOutputFile());
        }

        [Fact]
        internal void GetOutputFile_ReturnsExistingFileNameDuplicated()
        {
            _userSettings.RecordRecordingsStatus = Enums.RecordRecordingsStatus.Duplicate;
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\path\Artist - Title - Live.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem);

            var outputFile = _fileManager.GetOutputFile();

            Assert.Equal(@"C:\path\Artist - Title - Live 2.spytify", outputFile.ToPendingFileString());
            Assert.Equal(@"C:\path\Artist - Title - Live 2.mp3", outputFile.ToString());
        }

        [Fact]
        internal void GetOutputFile_ReturnsExistingFileNameNextDuplicated()
        {
            _userSettings.RecordRecordingsStatus = Enums.RecordRecordingsStatus.Duplicate;
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\path\Artist - Title - Live.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) },
                { @"C:\path\Artist - Title - Live 2.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) },
                { @"C:\path\Artist - Title - Live 3.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) },
                { @"C:\path\Artist - Title - Live 5.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem);

            var outputFile = _fileManager.GetOutputFile();

            Assert.Equal(@"C:\path\Artist - Title - Live 4.spytify", outputFile.ToPendingFileString());
            Assert.Equal(@"C:\path\Artist - Title - Live 4.mp3", outputFile.ToString());
        }

        [Fact]
        internal void GetOutputFile_ReturnsExistingFileNameToOverwrite()
        {
            _userSettings.RecordRecordingsStatus = Enums.RecordRecordingsStatus.Overwrite;
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\path\Artist - Title - Live.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem);

            var outputFile = _fileManager.GetOutputFile();

            Assert.Equal(@"C:\path\Artist - Title - Live.spytify", outputFile.ToPendingFileString());
            Assert.Equal(@"C:\path\Artist - Title - Live.mp3", outputFile.ToString());
        }

        [Fact]
        internal void BuildFileName_ReturnsFileName()
        {
            var fileName = _fileManager.GetOutputFile().ToString();

            Assert.Equal(@"C:\path\Artist - Title - Live.mp3", fileName);
        }

        [Fact]
        internal void BuildFileName_ReturnsFileNameOrderNumbered()
        {
            _userSettings.OrderNumberInfrontOfFileEnabled = true;
            _userSettings.TrackTitleSeparator = "_";
            _userSettings.InternalOrderNumber = 100;

            var fileName = _fileManager.GetOutputFile().ToString();

            Assert.Equal(@"C:\path\100_Artist_-_Title_-_Live.mp3", fileName);
        }

        [Fact]
        internal void BuildFileName_ReturnsFileNameGroupByFolders()
        {
            _userSettings.GroupByFoldersEnabled = true;

            var expected = @"C:\path\Artist\Single\Title - Live.mp3";
            var fileName = _fileManager.GetOutputFile().ToString();

            Assert.Equal(expected, fileName);
            Assert.True(_fileSystem.Directory.Exists(_fileSystem.Path.GetDirectoryName(expected)));
        }

        [Fact]
        internal void BuilFileName_ReturnsFileNameGroupByFoldersWhenUntitledAlbum()
        {
            _track.Album = "";
            _userSettings.GroupByFoldersEnabled = true;

            var expected = @"C:\path\Artist\Untitled\Title - Live.mp3";
            var fileName = _fileManager.GetOutputFile().ToString();

            Assert.Equal(expected, fileName);
            Assert.True(_fileSystem.Directory.Exists(_fileSystem.Path.GetDirectoryName(expected)));
        }

        [Fact]
        internal void BuildFileName_ReturnsFileNameWav()
        {
            _userSettings.MediaFormat = Enums.MediaFormat.Wav;

            var fileName = _fileManager.GetOutputFile().ToString();

            Assert.Equal(@"C:\path\Artist - Title - Live.wav", fileName);
        }

        [Fact]
        internal void GetFolderPath_ReturnsNoArtistFolderPath()
        {
            var artistFolder = FileManager.GetFolderPath(_track, _userSettings);

            Assert.Null(artistFolder);
        }

        [Fact]
        internal void GetFolderPath_ReturnsArtistFolderPath()
        {
            _userSettings.GroupByFoldersEnabled = true;

            var artistDir = Regex.Replace(_track.Artist, _windowsExlcudedChars, string.Empty);
            var albumDir = string.IsNullOrEmpty(_track.Album) ? Track.UNTITLED_ALBUM : Regex.Replace(_track.Album, _windowsExlcudedChars, string.Empty);

            var artistFolder = FileManager.GetFolderPath(_track, _userSettings);

            Assert.Equal($@"\{artistDir}\{albumDir}", artistFolder);
        }

        [Fact]
        internal void IsPathFileNameExists_ReturnsExists()
        {
            var result = FileManager.IsPathFileNameExists(_track, _userSettings, _fileSystem);
            Assert.False(result);
        }

        [Fact]
        internal void RenameFile_MoveFileToDestination()
        {
            var outputFile = new OutputFile
            {
                Path = _userSettings.OutputPath,
                File = _track.ToString(),
                Extension = _userSettings.MediaFormat.ToString().ToLower(),
                Separator = _userSettings.TrackTitleSeparator
            };

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\path\Artist - Title - Live.spytify", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem);

            _fileManager.RenameFile(outputFile.ToPendingFileString(), outputFile.ToString());

            Assert.Equal(@"C:\path\Artist - Title - Live.mp3", outputFile.ToString());
            Assert.True(_fileSystem.File.Exists(@"C:\path\Artist - Title - Live.mp3"));
        }

        [Fact]
        internal void RenameFile_MoveFileToDestinationAndOverwrite()
        {
            var outputFile = new OutputFile
            {
                Path = _userSettings.OutputPath,
                File = _track.ToString(),
                Extension = _userSettings.MediaFormat.ToString().ToLower(),
                Separator = _userSettings.TrackTitleSeparator
            };

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\path\Artist - Title - Live.spytify", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) },
                { @"C:\path\Artist - Title - Live.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem);

            _fileManager.RenameFile(outputFile.ToPendingFileString(), outputFile.ToString());

            Assert.Equal(@"C:\path\Artist - Title - Live.mp3", outputFile.ToString());
            Assert.True(_fileSystem.File.Exists(@"C:\path\Artist - Title - Live.mp3"));
        }

        [Fact]
        internal void RenameFile_CantMoveFileWhenNotFound()
        {
            var outputFile = new OutputFile
            {
                Path = _userSettings.OutputPath,
                File = _track.ToString(),
                Extension = _userSettings.MediaFormat.ToString().ToLower(),
                Separator = _userSettings.TrackTitleSeparator
            };

            _fileManager.RenameFile(outputFile.ToPendingFileString(), outputFile.ToString());

            Assert.Equal(@"C:\path\Artist - Title - Live.spytify", outputFile.ToPendingFileString());
            Assert.Equal(@"C:\path\Artist - Title - Live.mp3", outputFile.ToString());
            Assert.False(_fileSystem.File.Exists(@"C:\path\Artist - Title - Live.spytify"));
            Assert.False(_fileSystem.File.Exists(@"C:\path\Artist - Title - Live.mp3"));
        }

        [Fact]
        internal void DeleteFile_DeletesNoFileAndTrackNotFound()
        {
            var outputFile = new OutputFile
            {
                Path = _userSettings.OutputPath,
                File = _track.ToString(),
                Extension = _userSettings.MediaFormat.ToString().ToLower(),
                Separator = _userSettings.TrackTitleSeparator
            };

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\path\Artist - Title.wav", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem);

            _fileManager.DeleteFile(outputFile.ToPendingFileString());

            Assert.True(_fileSystem.File.Exists(@"C:\path\Artist - Title.wav"));
            Assert.False(_fileSystem.File.Exists(outputFile.ToPendingFileString()));
        }

        [Fact]
        internal void DeleteFile_DeletesFile()
        {
            _track.Title = "Delete_Me";
            _track.TitleExtended = "";
            _userSettings.TrackTitleSeparator = "_";

            var outputFile = new OutputFile
            {
                Path = _userSettings.OutputPath,
                File = _track.ToString(),
                Extension = _userSettings.MediaFormat.ToString().ToLower(),
                Separator = _userSettings.TrackTitleSeparator
            };

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\path\Artist_-_Delete_Me.spytify", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem);

            _fileManager.DeleteFile(outputFile.ToPendingFileString());

            Assert.False(_fileSystem.File.Exists(outputFile.ToPendingFileString()));
        }

        [Fact]
        internal void DeleteFile_DeletesNoFolderWhenGroupByFoldersEnabledAndTrackNotFound()
        {
            _userSettings.GroupByFoldersEnabled = true;

            var artistDir = Regex.Replace(_track.Artist, _windowsExlcudedChars, string.Empty);
            var outputFile = new OutputFile
            {
                Path = $@"{_userSettings.OutputPath}\{artistDir}",
                File = _track.ToTitleString(),
                Extension = _userSettings.MediaFormat.ToString().ToLower(),
                Separator = _userSettings.TrackTitleSeparator
            };

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\path\Artist", new MockDirectoryData() },
                { @"C:\path\Artist\Title.wav", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem);

            _fileManager.DeleteFile(outputFile.ToPendingFileString());

            Assert.True(_fileSystem.File.Exists(@"C:\path\Artist\Title.wav"));
            Assert.True(_fileSystem.Directory.Exists(@"C:\path\Artist"));
            Assert.False(_fileSystem.File.Exists(outputFile.ToPendingFileString()));
        }

        [Fact]
        internal void DeleteFile_DeletesFolderWhenGroupByFoldersEnabled()
        {
            _userSettings.GroupByFoldersEnabled = true;
            _track.Title = "Delete_Me";
            _track.TitleExtended = "";
            _userSettings.TrackTitleSeparator = "_";

            var artistDir = Regex.Replace(_track.Artist, _windowsExlcudedChars, string.Empty);
            var outputFile = new OutputFile
            {
                Path = $@"{_userSettings.OutputPath}\{artistDir}",
                File = _track.ToTitleString(),
                Extension = _userSettings.MediaFormat.ToString().ToLower(),
                Separator = _userSettings.TrackTitleSeparator
            };

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\path\Artist", new MockDirectoryData() },
                { @"C:\path\Artist\Delete_Me.spytify", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem);

            _fileManager.DeleteFile(outputFile.ToPendingFileString());

            Assert.False(_fileSystem.Directory.Exists($@"{_userSettings.OutputPath}\{artistDir}"));
            Assert.False(_fileSystem.File.Exists(outputFile.ToPendingFileString()));
        }
    }
}
