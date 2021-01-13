using EspionSpotify.Enums;
using EspionSpotify.Models;
using EspionSpotify.Native;
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
        private readonly string _networkPath = @"\\path\home";

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
                RecordEverythingEnabled = false,
                InternalOrderNumber = 1
            };

            _track = new Track
            {
                Title = "Title",
                Artist = "Artist",
                TitleExtended = "Live",
                TitleExtendedSeparatorType = TitleSeparatorType.Dash,
                Album = "Single",
                Ad = false
            };

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
        }

        [Fact]
        internal void GetOutputFile_ReturnsFileName()
        {
            var outputFile = _fileManager.GetOutputFile();

            Assert.Equal(_path, outputFile.BasePath);
            Assert.Equal(_track.ToString(), outputFile.File);
            Assert.Equal(_userSettings.MediaFormat.ToString().ToLower(), outputFile.Extension);
            Assert.Equal(_userSettings.TrackTitleSeparator, outputFile.Separator);

            Assert.Equal($@"{_path}\Artist - Title - Live.spytify", outputFile.ToSpytifyFilePath());
            Assert.Equal($@"{_path}\Artist - Title - Live.mp3", outputFile.ToMediaFilePath());
        }

        [Fact]
        internal void GetOutputFile_Grouped_ReturnsFileName()
        {
            _userSettings.GroupByFoldersEnabled = true;
            var outputFile = _fileManager.GetOutputFile();

            Assert.Equal(_path, outputFile.BasePath);
            Assert.Equal(@"Artist\Single", outputFile.FoldersPath);
            Assert.Equal(_track.ToTitleString(), outputFile.File);
            Assert.Equal(_userSettings.MediaFormat.ToString().ToLower(), outputFile.Extension);
            Assert.Equal(_userSettings.TrackTitleSeparator, outputFile.Separator);

            Assert.Equal($@"{_path}\Artist\Single\Title - Live.spytify", outputFile.ToSpytifyFilePath());
            Assert.Equal($@"{_path}\Artist\Single\Title - Live.mp3", outputFile.ToMediaFilePath());
        }

        [Fact]
        internal void GetOutputFile_ReturnsExistingFileNameDuplicated()
        {
            _userSettings.RecordRecordingsStatus = Enums.RecordRecordingsStatus.Duplicate;
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{_path}\Artist - Title - Live.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);

            var outputFile = _fileManager.GetOutputFile();

            Assert.Equal($@"{_path}\Artist - Title - Live 2.spytify", outputFile.ToSpytifyFilePath());
            Assert.Equal($@"{_path}\Artist - Title - Live 2.mp3", outputFile.ToMediaFilePath());
        }

        [Fact]
        internal void GetOutputFile_ReturnsExistingFileNameNextDuplicated()
        {
            _userSettings.RecordRecordingsStatus = Enums.RecordRecordingsStatus.Duplicate;
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{_path}\Artist - Title - Live.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) },
                { $@"{_path}\Artist - Title - Live 2.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) },
                { $@"{_path}\Artist - Title - Live 3.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) },
                { $@"{_path}\Artist - Title - Live 5.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);

            var outputFile = _fileManager.GetOutputFile();

            Assert.Equal($@"{_path}\Artist - Title - Live 4.spytify", outputFile.ToSpytifyFilePath());
            Assert.Equal($@"{_path}\Artist - Title - Live 4.mp3", outputFile.ToMediaFilePath());
        }

        [Fact]
        internal void GetOutputFile_ReturnsExistingFileNameToOverwrite()
        {
            _userSettings.RecordRecordingsStatus = Enums.RecordRecordingsStatus.Overwrite;
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{_path}\Artist - Title - Live.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);

            var outputFile = _fileManager.GetOutputFile();

            Assert.Equal($@"{_path}\Artist - Title - Live.spytify", outputFile.ToSpytifyFilePath());
            Assert.Equal($@"{_path}\Artist - Title - Live.mp3", outputFile.ToMediaFilePath());
        }

        [Fact]
        internal void BuildFileName_ReturnsFileName()
        {
            var fileName = _fileManager.GetOutputFile().ToMediaFilePath();

            Assert.Equal($@"{_path}\Artist - Title - Live.mp3", fileName);
        }

        [Fact]
        internal void BuildFileName_ReturnsUnixFileName()
        {
            _userSettings.OutputPath = _networkPath;

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var fileName = _fileManager.GetOutputFile().ToMediaFilePath();

            Assert.Equal($@"{_networkPath}\Artist - Title - Live.mp3", fileName);
        }

        [Fact]
        internal void BuildFileName_WithBadlyFormattedPath_Throws()
        {
            _userSettings.OutputPath = _networkPath;
            _track.Artist = null;

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);

            var ex = Assert.Throws<Exception>(() => _fileManager.GetOutputFile().ToMediaFilePath());
            Assert.Equal("Cannot recognize this type of track.", ex.Message);
        }

        [Theory]
        [InlineData(0, @"C:\path\000_Artist_-_Title_-_Live.mp3")]
        [InlineData(100, @"C:\path\100_Artist_-_Title_-_Live.mp3")]
        [InlineData(12345, @"C:\path\999_Artist_-_Title_-_Live.mp3")]
        internal void BuildFileName_ReturnsFileNameOrderNumbered(int orderNumber, string expected)
        {
            _userSettings.OrderNumberInfrontOfFileEnabled = true;
            _userSettings.TrackTitleSeparator = "_";
            _userSettings.InternalOrderNumber = orderNumber;

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var fileName = _fileManager.GetOutputFile().ToMediaFilePath();

            Assert.Equal(expected, fileName);
        }

        [Theory]
        [InlineData(0, @"\\path\home\000_Artist_-_Title_-_Live.mp3")]
        [InlineData(100, @"\\path\home\100_Artist_-_Title_-_Live.mp3")]
        [InlineData(12345, @"\\path\home\999_Artist_-_Title_-_Live.mp3")]
        internal void BuildFileName_ReturnsUnixFileNameOrderNumbered(int orderNumber, string expected)
        {
            _userSettings.OrderNumberInfrontOfFileEnabled = true;
            _userSettings.TrackTitleSeparator = "_";
            _userSettings.OutputPath = _networkPath;
            _userSettings.InternalOrderNumber = orderNumber;

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var fileName = _fileManager.GetOutputFile().ToMediaFilePath();

            Assert.Equal(expected, fileName);
        }

        [Fact]
        internal void BuildFileName_ReturnsFileNameWhenOrderNumberedIsDisabled()
        {
            _userSettings.OrderNumberInfrontOfFileEnabled = false;

            _userSettings.OrderNumberInMediaTagEnabled = true;
            _userSettings.InternalOrderNumber = 100;

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var fileName = _fileManager.GetOutputFile().ToMediaFilePath();

            Assert.Equal($@"{_path}\Artist - Title - Live.mp3", fileName);
        }

        [Fact]
        internal void BuildFileName_ReturnsFileNameGroupByFolders()
        {
            _userSettings.GroupByFoldersEnabled = true;

            var expected = $@"{_path}\Artist\Single\Title - Live.mp3";

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var fileName = _fileManager.GetOutputFile().ToMediaFilePath();

            Assert.Equal(expected, fileName);
            Assert.True(_fileSystem.Directory.Exists(_fileSystem.Path.GetDirectoryName(expected)));
        }

        [Fact]
        internal void BuildFileName_ReturnsUnixFileNameGroupByFolders()
        {
            _userSettings.GroupByFoldersEnabled = true;
            _userSettings.OutputPath = _networkPath;

            var expected = $@"{_networkPath}\Artist\Single\Title - Live.mp3";

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var fileName = _fileManager.GetOutputFile().ToMediaFilePath();

            Assert.Equal(expected, fileName);
            Assert.True(_fileSystem.Directory.Exists(_fileSystem.Path.GetDirectoryName(expected)));
        }

        [Fact]
        internal void BuildFileName_WithUnknownTrackWithGroupPath_ReturnsFileNameWithoutFolder()
        {
            _userSettings.GroupByFoldersEnabled = true;
            _track.Artist = "123 Podcast";
            _track.Title = null;

            var expected = $@"{_path}\{_track.Artist}.mp3";

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);

            var fileName = _fileManager.GetOutputFile().ToMediaFilePath();
            Assert.Equal(expected, fileName);
        }

        [Fact]
        internal void BuildFileName_WithBadlyFormattedArtistGroupPath_Throws()
        {
            _userSettings.GroupByFoldersEnabled = true;
            _track.Title = null;
            _track.Artist = "\\";

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);

            var ex = Assert.Throws<Exception>(() => _fileManager.GetOutputFile().ToMediaFilePath());
            Assert.Equal("File name cannot be empty.", ex.Message);
        }

        [Fact]
        internal void BuildFileName_WithBadlyFormattedTitleGroupPath_Throws()
        {
            _userSettings.GroupByFoldersEnabled = true;
            _track.Title = "?";
            _track.Artist = null;

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);

            var ex = Assert.Throws<Exception>(() => _fileManager.GetOutputFile().ToMediaFilePath());
            Assert.Equal("One or more directories has no name.", ex.Message);
        }

        [Fact]
        internal void BuilFileName_ReturnsFileNameGroupByFoldersWhenUntitledAlbum()
        {
            _track.Album = "";
            _userSettings.GroupByFoldersEnabled = true;

            var expected = $@"{_path}\Artist\Untitled\Title - Live.mp3";

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var fileName = _fileManager.GetOutputFile().ToMediaFilePath();

            Assert.Equal(expected, fileName);
            Assert.True(_fileSystem.Directory.Exists(_fileSystem.Path.GetDirectoryName(expected)));
        }

        [Fact]
        internal void BuilFileName_ReturnsUnixFileNameGroupByFoldersWhenUntitledAlbum()
        {
            _track.Album = "";
            _userSettings.OutputPath = _networkPath;
            _userSettings.GroupByFoldersEnabled = true;

            var expected = $@"{_networkPath}\Artist\Untitled\Title - Live.mp3";

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var fileName = _fileManager.GetOutputFile().ToMediaFilePath();

            Assert.Equal(expected, fileName);
            Assert.True(_fileSystem.Directory.Exists(_fileSystem.Path.GetDirectoryName(expected)));
        }

        [Fact]
        internal void BuildFileName_ReturnsFileNameWav()
        {
            _userSettings.MediaFormat = Enums.MediaFormat.Wav;

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var fileName = _fileManager.GetOutputFile().ToMediaFilePath();

            Assert.Equal($@"{_path}\Artist - Title - Live.wav", fileName);
        }

        [Fact]
        internal void BuildFileName_ReturnsUnixFileNameWav()
        {
            _userSettings.OutputPath = _networkPath;
            _userSettings.MediaFormat = Enums.MediaFormat.Wav;

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var fileName = _fileManager.GetOutputFile().ToMediaFilePath();

            Assert.Equal($@"{_networkPath}\Artist - Title - Live.wav", fileName);
        }

        [Fact]
        internal void GetFolderPath_ReturnsNoArtistFolderPath()
        {
            var artistFolder = FileManager.GetFolderPath(_track, _userSettings);

            Assert.Null(artistFolder);
        }

        [Fact]
        internal void GetFolderPath_WithoutArtist_Throws()
        {
            _userSettings.GroupByFoldersEnabled = true;
            _track.Artist = null;

            var exception = Assert.Throws<Exception>(() => FileManager.GetFolderPath(_track, _userSettings));

            Assert.Equal("Artist / Album cannot be null.", exception.Message);
        }

        [Fact]
        internal void GetFolderPath_OfUnknownTrack_GoesToOutputRoot()
        {
            _userSettings.GroupByFoldersEnabled = true;
            var track = new Track()
            {
                Artist = "Podcast",
                Ad = true,
            };

            var folders = FileManager.GetFolderPath(track, _userSettings);

            Assert.Null(folders);
        }

        [Fact]
        internal void GetFolderPath_ReturnsArtistAlbumFolderPath()
        {
            _userSettings.GroupByFoldersEnabled = true;
            _userSettings.TrackTitleSeparator = "_";
            _track.Artist = "Artist DJ";
            _track.Year = 2020;

            var folders = FileManager.GetFolderPath(_track, _userSettings);

            Assert.Equal($@"Artist_DJ\Single_(2020)", folders);
            Assert.Contains(_userSettings.TrackTitleSeparator, folders);
        }

        [Fact]
        internal void IsPathFileNameExists_ReturnsNotFound()
        {
            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var result = _fileManager.IsPathFileNameExists(_track, _userSettings, _fileSystem);
            Assert.False(result);
        }

        [Fact]
        internal void IsPathFileNameExists_ReturnsFound()
        {
            var track = new Track()
            {
                Artist = "Artist",
                Title = "Find Me",
                Album = "Single",
                Year = 2020,
            };

            _userSettings.MediaFormat = MediaFormat.Mp3;
            _userSettings.GroupByFoldersEnabled = true;

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{_path}\Artist", new MockDirectoryData() },
                { $@"{_path}\Artist\Single", new MockDirectoryData() },
                { $@"{_path}\Artist\Single (2020)\Find Me.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, track, _fileSystem, DateTime.Now);
            
            var result = _fileManager.IsPathFileNameExists(track, _userSettings, _fileSystem);
            Assert.True(result);
        }

        [Fact]
        internal void RenameFile_MoveFileToDestination()
        {
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{_path}\Artist - Title - Live.spytify", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var outputFile = _fileManager.GetOutputFile();

            _fileManager.RenameFile(outputFile.ToSpytifyFilePath(), outputFile.ToMediaFilePath());

            Assert.Equal($@"{_path}\Artist - Title - Live.mp3", outputFile.ToMediaFilePath());
            Assert.True(_fileSystem.File.Exists($@"{_path}\Artist - Title - Live.mp3"));
        }

        [Fact]
        internal void RenameFile_WithUnixFormattedPath_MoveFileToDestination()
        {
            _userSettings.OutputPath = _networkPath;

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{_networkPath}\Artist - Title - Live.spytify", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var outputFile = _fileManager.GetOutputFile();

            _fileManager.RenameFile(outputFile.ToSpytifyFilePath(), outputFile.ToMediaFilePath());

            Assert.Equal($@"{_networkPath}\Artist - Title - Live.mp3", outputFile.ToMediaFilePath());
            Assert.True(_fileSystem.File.Exists($@"{_networkPath}\Artist - Title - Live.mp3"));
        }

        [Fact]
        internal void RenameFile_MoveFileToDestinationAndOverwrite()
        {
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{_path}\Artist - Title - Live.spytify", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) },
                { $@"{_path}\Artist - Title - Live.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var outputFile = _fileManager.GetOutputFile();

            _fileManager.RenameFile(outputFile.ToSpytifyFilePath(), outputFile.ToMediaFilePath());

            Assert.Equal($@"{_path}\Artist - Title - Live.mp3", outputFile.ToMediaFilePath());
            Assert.True(_fileSystem.File.Exists($@"{_path}\Artist - Title - Live.mp3"));
        }

        [Fact]
        internal void RenameFile_WithInvalidFileName_Throws()
        {
            _track.Artist = null;

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{_path}\Artist - Title - Live.spytify", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) },
            });

            var outputFile = new OutputFile
            {
                FoldersPath = _userSettings.OutputPath,
                File = _track.ToString(),
                Extension = _userSettings.MediaFormat.ToString().ToLower(),
                Separator = _userSettings.TrackTitleSeparator
            };

            var ex = Assert.Throws<Exception>(() => _fileManager.RenameFile(outputFile.ToSpytifyFilePath(), outputFile.ToMediaFilePath()));
            Assert.Equal("Source / Destination file name cannot be null.", ex.Message);

            Assert.True(_fileSystem.File.Exists($@"{_path}\Artist - Title - Live.spytify"));
            Assert.False(_fileSystem.File.Exists($@"{_path}\Artist - Title - Live.mp3"));
        }

        [Fact]
        internal void RenameFile_RenamesNothingWhenGroupByFoldersEnabledAndTrackNotFound()
        {
            _userSettings.GroupByFoldersEnabled = true;

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{_path}\Artist", new MockDirectoryData() },
                { $@"{_path}\Artist\Album", new MockDirectoryData() },
                { $@"{_path}\Artist\Album\Title.wav", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var outputFile = _fileManager.GetOutputFile();

            Assert.False(_fileSystem.File.Exists(outputFile.ToSpytifyFilePath()));

            _fileManager.RenameFile(outputFile.ToSpytifyFilePath(), outputFile.ToMediaFilePath());

            Assert.True(_fileSystem.File.Exists($@"{_path}\Artist\Album\Title.wav"));
            Assert.True(_fileSystem.Directory.Exists($@"{_path}\Artist\Album"));
            Assert.True(_fileSystem.Directory.Exists($@"{_path}\Artist"));
            Assert.False(_fileSystem.File.Exists(outputFile.ToSpytifyFilePath()));
            Assert.False(_fileSystem.File.Exists(outputFile.ToMediaFilePath()));
        }

        [Fact]
        internal void RenameFile_RenamesFileWhenGroupByFoldersEnabled()
        {
            _userSettings.GroupByFoldersEnabled = true;
            _userSettings.MediaFormat = MediaFormat.Mp3;
            _track.Title = "Rename_Me";
            _track.TitleExtended = "";
            _userSettings.TrackTitleSeparator = "_";

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{_path}\Artist", new MockDirectoryData() },
                { $@"{_path}\Artist\Single", new MockDirectoryData() },
                { $@"{_path}\Artist\Single\Rename_Me.spytify", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var outputFile = _fileManager.GetOutputFile();

            Assert.True(_fileSystem.File.Exists(outputFile.ToSpytifyFilePath()));

            _fileManager.RenameFile(outputFile.ToSpytifyFilePath(), outputFile.ToMediaFilePath());

            Assert.True(_fileSystem.Directory.Exists($@"{_path}\Artist"));
            Assert.True(_fileSystem.Directory.Exists($@"{_path}\Artist\Single"));
            Assert.False(_fileSystem.File.Exists(outputFile.ToSpytifyFilePath()));
            Assert.True(_fileSystem.File.Exists(outputFile.ToMediaFilePath()));
        }

        [Fact]
        internal void RenameFile_RenamesUnixFileWhenGroupByFoldersEnabled()
        {
            _userSettings.GroupByFoldersEnabled = true;
            _userSettings.MediaFormat = MediaFormat.Mp3;
            _userSettings.OutputPath = _networkPath;
            _track.Title = "Rename_Me";
            _track.TitleExtended = "";
            _userSettings.TrackTitleSeparator = "_";

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{_networkPath}\Artist", new MockDirectoryData() },
                { $@"{_networkPath}\Artist\Single", new MockDirectoryData() },
                { $@"{_networkPath}\Artist\Single\Rename_Me.spytify", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var outputFile = _fileManager.GetOutputFile();

            Assert.True(_fileSystem.File.Exists(outputFile.ToSpytifyFilePath()));

            _fileManager.RenameFile(outputFile.ToSpytifyFilePath(), outputFile.ToMediaFilePath());

            Assert.True(_fileSystem.Directory.Exists($@"{_networkPath}\Artist"));
            Assert.True(_fileSystem.Directory.Exists($@"{_networkPath}\Artist\Single"));
            Assert.False(_fileSystem.File.Exists(outputFile.ToSpytifyFilePath()));
            Assert.True(_fileSystem.File.Exists(outputFile.ToMediaFilePath()));
        }

        [Fact]
        internal void RenameFile_CantMoveFileWhenNotFound()
        {
            var outputFile = _fileManager.GetOutputFile();
            _fileManager.RenameFile(outputFile.ToSpytifyFilePath(), outputFile.ToMediaFilePath());

            Assert.Equal($@"{_path}\Artist - Title - Live.spytify", outputFile.ToSpytifyFilePath());
            Assert.Equal($@"{_path}\Artist - Title - Live.mp3", outputFile.ToMediaFilePath());
            Assert.False(_fileSystem.File.Exists($@"{_path}\Artist - Title - Live.spytify"));
            Assert.False(_fileSystem.File.Exists($@"{_path}\Artist - Title - Live.mp3"));
        }

        [Fact]
        internal void DeleteFile_DeletesNoFileAndTrackNotFound()
        {
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{_path}\Artist - Title.wav", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var outputFile = _fileManager.GetOutputFile();

            Assert.False(_fileSystem.File.Exists(outputFile.ToSpytifyFilePath()));

            _fileManager.DeleteFile(outputFile.ToSpytifyFilePath());

            Assert.True(_fileSystem.File.Exists($@"{_path}\Artist - Title.wav"));
            Assert.False(_fileSystem.File.Exists(outputFile.ToSpytifyFilePath()));
        }

        [Fact]
        internal void DeleteFile_DeletesFile()
        {
            _track.Title = "Delete_Me";
            _track.TitleExtended = "";
            _userSettings.TrackTitleSeparator = "_";

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{_path}\Artist_-_Delete_Me.spytify", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var outputFile = _fileManager.GetOutputFile();

            Assert.True(_fileSystem.File.Exists(outputFile.ToSpytifyFilePath()));

            _fileManager.DeleteFile(outputFile.ToSpytifyFilePath());

            Assert.False(_fileSystem.File.Exists(outputFile.ToSpytifyFilePath()));
        }

        [Fact]
        internal void DeleteFile_WithUnixFormattedPath_DeletesFile()
        {
            _track.Title = "Delete Me";
            _track.TitleExtended = "";
            _track.Album = null;
            _userSettings.GroupByFoldersEnabled = true;
            _userSettings.TrackTitleSeparator = " ";
            _userSettings.OutputPath = _networkPath;

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{_networkPath}\Artist\Untitled\Delete Me.spytify", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var outputFile = _fileManager.GetOutputFile();

            Assert.True(_fileSystem.File.Exists(outputFile.ToSpytifyFilePath()));

            _fileManager.DeleteFile(outputFile.ToSpytifyFilePath());

            Assert.False(_fileSystem.File.Exists(outputFile.ToSpytifyFilePath()));
        }

        [Fact]
        internal void DeleteFile_WithInvalidFileName_Throws()
        {
            _track.Title = "Delete Me";
            _track.TitleExtended = "";
            _track.Artist = null;

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{_path}\Artist - Delete Me.spytify", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            var outputFile = new OutputFile
            {
                FoldersPath = _userSettings.OutputPath,
                File = _track.ToString(),
                Extension = _userSettings.MediaFormat.ToString().ToLower(),
                Separator = _userSettings.TrackTitleSeparator
            };

            var ex = Assert.Throws<Exception>(() => _fileManager.DeleteFile(outputFile.ToSpytifyFilePath()));
            Assert.Equal("File name cannot be null.", ex.Message);
        }

        [Fact]
        internal void DeleteFile_DeletesNoFolderWhenGroupByFoldersEnabledAndTrackNotFound()
        {
            _userSettings.GroupByFoldersEnabled = true;

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{_path}\Artist", new MockDirectoryData() },
                { $@"{_path}\Artist\Album", new MockDirectoryData() },
                { $@"{_path}\Artist\Album\Title.wav", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var outputFile = _fileManager.GetOutputFile();

            Assert.False(_fileSystem.File.Exists(outputFile.ToSpytifyFilePath()));

            _fileManager.DeleteFile(outputFile.ToSpytifyFilePath());

            Assert.True(_fileSystem.File.Exists($@"{_path}\Artist\Album\Title.wav"));
            Assert.True(_fileSystem.Directory.Exists($@"{_path}\Artist\Album"));
            Assert.True(_fileSystem.Directory.Exists($@"{_path}\Artist"));
            Assert.False(_fileSystem.File.Exists(outputFile.ToSpytifyFilePath()));
        }

        [Fact]
        internal void DeleteFile_DeletesFileAndFolderWhenGroupByFoldersEnabled()
        {
            _userSettings.GroupByFoldersEnabled = true;
            _userSettings.MediaFormat = MediaFormat.Mp3;
            _track.Title = "Delete_Me";
            _track.TitleExtended = "";
            _userSettings.TrackTitleSeparator = "_";

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{_path}\Artist\Single\Delete_Me.spytify", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var outputFile = _fileManager.GetOutputFile();

            Assert.True(_fileSystem.File.Exists(outputFile.ToSpytifyFilePath()));

            _fileManager.DeleteFile(outputFile.ToSpytifyFilePath());

            Assert.False(_fileSystem.File.Exists(outputFile.ToSpytifyFilePath()));
        }

        [Fact]
        internal void DeleteFile_DeletesUnixFileAndFolderWhenGroupByFoldersEnabled()
        {
            _userSettings.GroupByFoldersEnabled = true;
            _userSettings.OutputPath = _networkPath;
            _userSettings.MediaFormat = MediaFormat.Mp3;
            _track.Title = "Delete_Me";
            _track.TitleExtended = "";
            _userSettings.TrackTitleSeparator = "_";

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{_networkPath}\Artist", new MockDirectoryData() },
                { $@"{_networkPath}\Artist\Single", new MockDirectoryData() },
                { $@"{_networkPath}\Artist\Single\Delete_Me.spytify", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var outputFile = _fileManager.GetOutputFile();

            Assert.True(_fileSystem.File.Exists(outputFile.ToSpytifyFilePath()));

            _fileManager.DeleteFile(outputFile.ToSpytifyFilePath());

            Assert.False(_fileSystem.Directory.Exists($@"{_networkPath}\Artist"));
            Assert.False(_fileSystem.Directory.Exists($@"{_networkPath}\Artist\Single"));
            Assert.False(_fileSystem.Directory.Exists($@"{_networkPath}\Artist\Single\Delete_Me.spytify"));
            Assert.False(_fileSystem.File.Exists(outputFile.ToSpytifyFilePath()));
        }

        [Fact]
        internal void DeleteFile_DeletesTempFile()
        {
            var tempFilePath = @"C:\Local\Temp\123.tmp";
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"C:\Local", new MockDirectoryData() },
                { tempFilePath, new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);

            Assert.True(_fileSystem.File.Exists(tempFilePath));

            _fileManager.DeleteFile(tempFilePath);

            Assert.True(_fileSystem.Directory.Exists(@"C:\local\Temp"));
            Assert.False(_fileSystem.File.Exists(tempFilePath));
        }

        [Fact]
        internal void UpdateOutputFileWithLatestTrackInfo_UpdateFile()
        {
            var outputFile = new OutputFile
            {
                FoldersPath = _userSettings.OutputPath,
                File = _track.ToString(),
                Extension = _userSettings.MediaFormat.ToString().ToLower(),
                Separator = _userSettings.TrackTitleSeparator
            };

            var latestTrack = new Track
            {
                Title = "Title",
                Artist = "Artist",
                TitleExtended = "Live",
                TitleExtendedSeparatorType = TitleSeparatorType.Dash,
                Album = "Single",
                Ad = false,
                AlbumArtists = new[] { "DJ" },
                Performers = new[] { "Artist", "Featuring" },
            };

            _fileManager.UpdateOutputFileWithLatestTrackInfo(outputFile, latestTrack, _userSettings);

            Assert.Equal("DJ - Title - Live", outputFile.File);
            Assert.Equal($@"{_path}\DJ - Title - Live.mp3", outputFile.ToMediaFilePath());
        }
    }
}
