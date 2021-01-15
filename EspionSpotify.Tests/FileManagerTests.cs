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

        private const string PATH = @"C:\path";
        private const string NETWORK_PATH = @"\\path\home";
        private const string TEMPORARY_FILE = "spytify.tmp";

        private readonly string _tempFileFullPath = $@"{PATH}\{TEMPORARY_FILE}";
        private readonly string _tempFileFullPathNetwork = $@"{NETWORK_PATH}\{TEMPORARY_FILE}";

        public FileManagerTests()
        {
            _userSettings = new UserSettings
            {
                OutputPath = PATH,
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

            Assert.Equal(PATH, outputFile.BasePath);
            Assert.Equal(_track.ToString(), outputFile.MediaFile);
            Assert.Equal(_userSettings.MediaFormat.ToString().ToLower(), outputFile.Extension);
            Assert.Equal(_userSettings.TrackTitleSeparator, outputFile.Separator);

            Assert.Equal($@"{PATH}\Artist - Title - Live.mp3", outputFile.ToMediaFilePath());
        }

        [Fact]
        internal void GetOutputFile_Grouped_ReturnsFileName()
        {
            _userSettings.GroupByFoldersEnabled = true;
            var outputFile = _fileManager.GetOutputFile();

            Assert.Equal(PATH, outputFile.BasePath);
            Assert.Equal(@"Artist\Single", outputFile.FoldersPath);
            Assert.Equal(_track.ToTitleString(), outputFile.MediaFile);
            Assert.Equal(_userSettings.MediaFormat.ToString().ToLower(), outputFile.Extension);
            Assert.Equal(_userSettings.TrackTitleSeparator, outputFile.Separator);

            Assert.Equal($@"..\Artist\Single\Title - Live.mp3", outputFile.ToString());
            Assert.Equal($@"{PATH}\Artist\Single\Title - Live.mp3", outputFile.ToMediaFilePath());
        }

        [Fact]
        internal void GetOutputFile_ReturnsExistingFileNameDuplicated()
        {
            _userSettings.RecordRecordingsStatus = Enums.RecordRecordingsStatus.Duplicate;
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{PATH}\Artist - Title - Live.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);

            var outputFile = _fileManager.GetOutputFile();

            Assert.Equal($@"..\Artist - Title - Live 2.mp3", outputFile.ToString());
            Assert.Equal($@"{PATH}\Artist - Title - Live 2.mp3", outputFile.ToMediaFilePath());
        }

        [Fact]
        internal void GetOutputFile_ReturnsExistingFileNameNextDuplicated()
        {
            _userSettings.RecordRecordingsStatus = Enums.RecordRecordingsStatus.Duplicate;
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{PATH}\Artist - Title - Live.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) },
                { $@"{PATH}\Artist - Title - Live 2.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) },
                { $@"{PATH}\Artist - Title - Live 3.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) },
                { $@"{PATH}\Artist - Title - Live 5.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);

            var outputFile = _fileManager.GetOutputFile();

            Assert.Equal($@"..\Artist - Title - Live 4.mp3", outputFile.ToString());
            Assert.Equal($@"{PATH}\Artist - Title - Live 4.mp3", outputFile.ToMediaFilePath());
        }

        [Fact]
        internal void GetOutputFile_ReturnsExistingFileNameToOverwrite()
        {
            _userSettings.RecordRecordingsStatus = Enums.RecordRecordingsStatus.Overwrite;
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{PATH}\Artist - Title - Live.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);

            var outputFile = _fileManager.GetOutputFile();

            Assert.Equal($@"..\Artist - Title - Live.mp3", outputFile.ToString());
            Assert.Equal($@"{PATH}\Artist - Title - Live.mp3", outputFile.ToMediaFilePath());
        }

        [Fact]
        internal void BuildFileName_ReturnsFileName()
        {
            var fileName = _fileManager.GetOutputFile().ToMediaFilePath();

            Assert.Equal($@"{PATH}\Artist - Title - Live.mp3", fileName);
        }

        [Fact]
        internal void BuildFileName_ReturnsUnixFileName()
        {
            _userSettings.OutputPath = NETWORK_PATH;

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var fileName = _fileManager.GetOutputFile().ToMediaFilePath();

            Assert.Equal($@"{NETWORK_PATH}\Artist - Title - Live.mp3", fileName);
        }

        [Fact]
        internal void BuildFileName_WithBadlyFormattedPath_Throws()
        {
            _userSettings.OutputPath = NETWORK_PATH;
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
            _userSettings.OutputPath = NETWORK_PATH;
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

            Assert.Equal($@"{PATH}\Artist - Title - Live.mp3", fileName);
        }

        [Fact]
        internal void BuildFileName_ReturnsFileNameGroupByFolders()
        {
            _userSettings.GroupByFoldersEnabled = true;

            var expected = $@"{PATH}\Artist\Single\Title - Live.mp3";

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var fileName = _fileManager.GetOutputFile().ToMediaFilePath();

            Assert.Equal(expected, fileName);
            Assert.True(_fileSystem.Directory.Exists(_fileSystem.Path.GetDirectoryName(expected)));
        }

        [Fact]
        internal void BuildFileName_ReturnsUnixFileNameGroupByFolders()
        {
            _userSettings.GroupByFoldersEnabled = true;
            _userSettings.OutputPath = NETWORK_PATH;

            var expected = $@"{NETWORK_PATH}\Artist\Single\Title - Live.mp3";

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

            var expected = $@"{PATH}\{_track.Artist}.mp3";

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

            var ex = Assert.Throws<Exception>(() => _fileManager.GetOutputFile());
            Assert.Equal("File name cannot be empty.", ex.Message);
        }

        [Fact]
        internal void BuildFileName_WithBadlyFormattedTitleGroupPath_Throws()
        {
            _userSettings.GroupByFoldersEnabled = true;
            _track.Title = "?";
            _track.Artist = "?";

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);

            var ex = Assert.Throws<Exception>(() => _fileManager.GetOutputFile());
            Assert.Equal("Artist / Album cannot be null.", ex.Message);
        }

        [Fact]
        internal void BuilFileName_ReturnsFileNameGroupByFoldersWhenUntitledAlbum()
        {
            _track.Album = "";
            _userSettings.GroupByFoldersEnabled = true;

            var expected = $@"{PATH}\Artist\Untitled\Title - Live.mp3";

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var fileName = _fileManager.GetOutputFile().ToMediaFilePath();

            Assert.Equal(expected, fileName);
            Assert.True(_fileSystem.Directory.Exists(_fileSystem.Path.GetDirectoryName(expected)));
        }

        [Fact]
        internal void BuilFileName_ReturnsUnixFileNameGroupByFoldersWhenUntitledAlbum()
        {
            _track.Album = "";
            _userSettings.OutputPath = NETWORK_PATH;
            _userSettings.GroupByFoldersEnabled = true;

            var expected = $@"{NETWORK_PATH}\Artist\Untitled\Title - Live.mp3";

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

            Assert.Equal($@"{PATH}\Artist - Title - Live.wav", fileName);
        }

        [Fact]
        internal void BuildFileName_ReturnsUnixFileNameWav()
        {
            _userSettings.OutputPath = NETWORK_PATH;
            _userSettings.MediaFormat = Enums.MediaFormat.Wav;

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var fileName = _fileManager.GetOutputFile().ToMediaFilePath();

            Assert.Equal($@"{NETWORK_PATH}\Artist - Title - Live.wav", fileName);
        }

        [Fact]
        internal void GetFolderPath_ReturnsNoArtistFolderPath()
        {
            var (artistFolder, albumFolder) = FileManager.GetFolderPath(_track, _userSettings);

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

            var (artistFoder, albumFolder) = FileManager.GetFolderPath(track, _userSettings);

            Assert.Null(artistFoder);
            Assert.Null(albumFolder);
        }

        [Fact]
        internal void GetFolderPath_ReturnsArtistAlbumFolderPath()
        {
            _userSettings.GroupByFoldersEnabled = true;
            _userSettings.TrackTitleSeparator = "_";
            _track.Artist = "Artist DJ";
            _track.Year = 2020;

            var (artistFoder, albumFolder) = FileManager.GetFolderPath(_track, _userSettings);
            var folders = FileManager.ConcatPaths(artistFoder, albumFolder);

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
        internal void IsPathFileNameExists_WithBadlyFormattedArtistGroupPath_Throws()
        {
            _userSettings.GroupByFoldersEnabled = true;
            _track.Title = null;
            _track.Artist = "\\";

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);

            var ex = Assert.Throws<Exception>(() => _fileManager.IsPathFileNameExists(_track, _userSettings, _fileSystem));
            Assert.Equal("File name cannot be empty.", ex.Message);
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
                { $@"{PATH}\Artist", new MockDirectoryData() },
                { $@"{PATH}\Artist\Single", new MockDirectoryData() },
                { $@"{PATH}\Artist\Single (2020)\Find Me.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
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
                { $@"{PATH}\{TEMPORARY_FILE}", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var outputFile = _fileManager.GetOutputFile();

            _fileManager.RenameFile(_tempFileFullPath, outputFile.ToMediaFilePath());

            Assert.Equal($@"{PATH}\Artist - Title - Live.mp3", outputFile.ToMediaFilePath());
            Assert.True(_fileSystem.File.Exists($@"{PATH}\Artist - Title - Live.mp3"));
        }

        [Fact]
        internal void RenameFile_WithNetworkFormattedPath_MoveFileToDestination()
        {
            _userSettings.OutputPath = NETWORK_PATH;

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{NETWORK_PATH}\{TEMPORARY_FILE}", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var outputFile = _fileManager.GetOutputFile();

            _fileManager.RenameFile(_tempFileFullPathNetwork, outputFile.ToMediaFilePath());

            Assert.Equal($@"{NETWORK_PATH}\Artist - Title - Live.mp3", outputFile.ToMediaFilePath());
            Assert.True(_fileSystem.File.Exists($@"{NETWORK_PATH}\Artist - Title - Live.mp3"));
        }

        [Fact]
        internal void RenameFile_MoveFileToDestinationAndOverwrite()
        {
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{PATH}\{TEMPORARY_FILE}", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) },
                { $@"{PATH}\Artist - Title - Live.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var outputFile = _fileManager.GetOutputFile();

            _fileManager.RenameFile(_tempFileFullPath, outputFile.ToMediaFilePath());

            Assert.Equal($@"{PATH}\Artist - Title - Live.mp3", outputFile.ToMediaFilePath());
            Assert.True(_fileSystem.File.Exists($@"{PATH}\Artist - Title - Live.mp3"));
        }

        [Fact]
        internal void RenameFile_WithInvalidFileName_Throws()
        {
            _track.Artist = null;

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{PATH}\{TEMPORARY_FILE}", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) },
            });

            var outputFile = new OutputFile
            {
                FoldersPath = _userSettings.OutputPath,
                MediaFile = _track.ToString(),
                Extension = _userSettings.MediaFormat.ToString().ToLower(),
                Separator = _userSettings.TrackTitleSeparator
            };

            var ex = Assert.Throws<Exception>(() => _fileManager.RenameFile(_tempFileFullPath, outputFile.ToMediaFilePath()));
            Assert.Equal("Source / Destination file name cannot be null.", ex.Message);

            Assert.True(_fileSystem.File.Exists(_tempFileFullPath));
            Assert.False(_fileSystem.File.Exists($@"{PATH}\Artist - Title - Live.mp3"));
        }

        [Fact]
        internal void RenameFile_RenamesNothingWhenGroupByFoldersEnabledAndTrackNotFound()
        {
            _userSettings.GroupByFoldersEnabled = true;

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{PATH}\Artist", new MockDirectoryData() },
                { $@"{PATH}\Artist\Album", new MockDirectoryData() },
                { $@"{PATH}\Artist\Album\Title.wav", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var outputFile = _fileManager.GetOutputFile();

            Assert.False(_fileSystem.File.Exists(_tempFileFullPath));

            _fileManager.RenameFile(_tempFileFullPath, outputFile.ToMediaFilePath());

            Assert.True(_fileSystem.File.Exists($@"{PATH}\Artist\Album\Title.wav"));
            Assert.True(_fileSystem.Directory.Exists($@"{PATH}\Artist\Album"));
            Assert.True(_fileSystem.Directory.Exists($@"{PATH}\Artist"));
            Assert.False(_fileSystem.File.Exists(_tempFileFullPath));
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
                { $@"{PATH}\{TEMPORARY_FILE}", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var outputFile = _fileManager.GetOutputFile();

            Assert.True(_fileSystem.File.Exists(_tempFileFullPath));

            _fileManager.RenameFile(_tempFileFullPath, outputFile.ToMediaFilePath());

            Assert.True(_fileSystem.Directory.Exists($@"{PATH}\Artist"));
            Assert.True(_fileSystem.Directory.Exists($@"{PATH}\Artist\Single"));
            Assert.False(_fileSystem.File.Exists(_tempFileFullPath));
            Assert.True(_fileSystem.File.Exists(outputFile.ToMediaFilePath()));
        }

        [Fact]
        internal void RenameFile_RenamesUnixFileWhenGroupByFoldersEnabled()
        {
            _userSettings.GroupByFoldersEnabled = true;
            _userSettings.MediaFormat = MediaFormat.Mp3;
            _userSettings.OutputPath = NETWORK_PATH;
            _track.Title = "Rename_Me";
            _track.TitleExtended = "";
            _userSettings.TrackTitleSeparator = "_";

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {$@"{NETWORK_PATH}\{TEMPORARY_FILE}", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var outputFile = _fileManager.GetOutputFile();

            Assert.True(_fileSystem.File.Exists(_tempFileFullPathNetwork));

            _fileManager.RenameFile(_tempFileFullPathNetwork, outputFile.ToMediaFilePath());

            Assert.True(_fileSystem.Directory.Exists($@"{NETWORK_PATH}\Artist"));
            Assert.True(_fileSystem.Directory.Exists($@"{NETWORK_PATH}\Artist\Single"));
            Assert.False(_fileSystem.File.Exists(_tempFileFullPathNetwork));
            Assert.True(_fileSystem.File.Exists(outputFile.ToMediaFilePath()));
        }

        [Fact]
        internal void RenameFile_CantMoveFileWhenNotFound()
        {
            var outputFile = _fileManager.GetOutputFile();
            _fileManager.RenameFile(_tempFileFullPath, outputFile.ToMediaFilePath());

            Assert.Equal($@"{PATH}\Artist - Title - Live.mp3", outputFile.ToMediaFilePath());
            Assert.False(_fileSystem.File.Exists(_tempFileFullPath));
            Assert.False(_fileSystem.File.Exists($@"{PATH}\Artist - Title - Live.mp3"));
        }

        [Fact]
        internal void DeleteFile_DeletesNoFileAndTrackNotFound()
        {
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{PATH}\Artist - Title.wav", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var outputFile = _fileManager.GetOutputFile();

            Assert.False(_fileSystem.File.Exists(_tempFileFullPath));

            _fileManager.DeleteFile(_tempFileFullPath);

            Assert.True(_fileSystem.File.Exists($@"{PATH}\Artist - Title.wav"));
            Assert.False(_fileSystem.File.Exists(_tempFileFullPath));
        }

        [Fact]
        internal void DeleteFile_DeletesFile()
        {
            _track.Title = "Delete_Me";
            _track.TitleExtended = "";
            _userSettings.TrackTitleSeparator = "_";

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{PATH}\{TEMPORARY_FILE}", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var outputFile = _fileManager.GetOutputFile();

            Assert.True(_fileSystem.File.Exists(_tempFileFullPath));

            _fileManager.DeleteFile(_tempFileFullPath);

            Assert.False(_fileSystem.File.Exists(_tempFileFullPath));
        }

        [Fact]
        internal void DeleteFile_WithUnixFormattedPath_DeletesFile()
        {
            _track.Title = "Delete Me";
            _track.TitleExtended = "";
            _track.Album = null;
            _userSettings.GroupByFoldersEnabled = true;
            _userSettings.TrackTitleSeparator = " ";
            _userSettings.OutputPath = NETWORK_PATH;

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{NETWORK_PATH}\Artist", new MockDirectoryData() },
                { $@"{NETWORK_PATH}\Artist\Untitled", new MockDirectoryData() },
                { $@"{NETWORK_PATH}\Artist\Untitled\Delete Me.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var outputFile = _fileManager.GetOutputFile();

            Assert.True(_fileSystem.File.Exists(outputFile.ToMediaFilePath()));

            _fileManager.DeleteFile(outputFile.ToMediaFilePath());

            Assert.False(_fileSystem.File.Exists(outputFile.ToMediaFilePath()));
        }

        [Fact]
        internal void DeleteFile_WithInvalidFileName_Throws()
        {
            _track.Title = "Delete Me";
            _track.TitleExtended = "";
            _track.Artist = null;

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{PATH}\Artist", new MockDirectoryData() },
                { $@"{PATH}\Artist\Single", new MockDirectoryData() },
                { $@"{PATH}\Artist\Single\Delete Me.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            var outputFile = new OutputFile
            {
                FoldersPath = _userSettings.OutputPath,
                MediaFile = _track.ToString(),
                Extension = _userSettings.MediaFormat.ToString().ToLower(),
                Separator = _userSettings.TrackTitleSeparator
            };

            var ex = Assert.Throws<Exception>(() => _fileManager.DeleteFile(outputFile.ToMediaFilePath()));
            Assert.Equal("File name cannot be null.", ex.Message);
        }

        [Fact]
        internal void DeleteFile_DeletesNoFolderWhenGroupByFoldersEnabledAndTrackNotFound()
        {
            _userSettings.GroupByFoldersEnabled = true;

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{PATH}\Artist", new MockDirectoryData() },
                { $@"{PATH}\Artist\Album", new MockDirectoryData() },
                { $@"{PATH}\Artist\Album\Title.wav", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var outputFile = _fileManager.GetOutputFile();

            Assert.False(_fileSystem.File.Exists(_tempFileFullPath));

            _fileManager.DeleteFile(_tempFileFullPath);

            Assert.True(_fileSystem.File.Exists($@"{PATH}\Artist\Album\Title.wav"));
            Assert.True(_fileSystem.Directory.Exists($@"{PATH}\Artist\Album"));
            Assert.True(_fileSystem.Directory.Exists($@"{PATH}\Artist"));
            Assert.False(_fileSystem.File.Exists(_tempFileFullPath));
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
                { $@"{PATH}\Artist", new MockDirectoryData() },
                { $@"{PATH}\Artist\Single", new MockDirectoryData() },
                { $@"{PATH}\Artist\Single\Delete_Me.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var outputFile = _fileManager.GetOutputFile();

            Assert.True(_fileSystem.File.Exists(outputFile.ToMediaFilePath()));

            _fileManager.DeleteFile(outputFile.ToMediaFilePath());

            Assert.False(_fileSystem.File.Exists(outputFile.ToMediaFilePath()));
        }

        [Fact]
        internal void DeleteFile_DeletesNetworkFileAndFolderWhenGroupByFoldersEnabled()
        {
            _userSettings.GroupByFoldersEnabled = true;
            _userSettings.OutputPath = NETWORK_PATH;
            _userSettings.MediaFormat = MediaFormat.Mp3;
            _track.Title = "Delete_Me";
            _track.TitleExtended = "";
            _userSettings.TrackTitleSeparator = "_";

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $@"{NETWORK_PATH}\Artist", new MockDirectoryData() },
                { $@"{NETWORK_PATH}\Artist\Single", new MockDirectoryData() },
                { $@"{NETWORK_PATH}\Artist\Single\Delete_Me.mp3", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            _fileManager = new FileManager(_userSettings, _track, _fileSystem, DateTime.Now);
            var outputFile = _fileManager.GetOutputFile();

            Assert.True(_fileSystem.File.Exists(outputFile.ToMediaFilePath()));

            _fileManager.DeleteFile(outputFile.ToMediaFilePath());

            Assert.False(_fileSystem.Directory.Exists($@"{NETWORK_PATH}\Artist"));
            Assert.False(_fileSystem.Directory.Exists($@"{NETWORK_PATH}\Artist\Single"));
            Assert.False(_fileSystem.Directory.Exists($@"{NETWORK_PATH}\{_tempFileFullPath}"));
            Assert.False(_fileSystem.File.Exists(outputFile.ToMediaFilePath()));
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

        [Theory]
        [InlineData(@" C:\path\ ", @"C:\path")]
        [InlineData(@"C:\path\\", @"C:\path")]
        [InlineData(@"\\pa<>|th", @"\\path")]
        [InlineData(@"\\p?ath?", @"\\p?ath")] // trim removes ?, regex keep ? not invalid in path
        [InlineData(@"\\PATH path\", @"\\PATH path")]
        [InlineData(@"\\path\path\", @"\\path\path")]
        internal void GetCleanPath_ReturnsPathCleaner(string value, string expected)
        {
            Assert.Equal(expected, FileManager.GetCleanPath(value));
        }

        [Theory]
        [InlineData(@" Full Artist Name ", @"Full Artist Name")]
        [InlineData(@" Full_Artist_Name ", @"Full_Artist_Name")]
        [InlineData(@"Artist\Title", @"ArtistTitle")]
        [InlineData(@"R?pertoire", @"Rpertoire")]
        [InlineData(@"\\fold?<>|*://er", @"folder")]
        [InlineData(@"\\répertoire\", @"répertoire")]
        [InlineData(@"W!,.z1K-", @"W!,.z1K-")]
        internal void GetCleanFileFolder_ReturnsFileFolderCleaned(string value, string expected)
        {
            Assert.Equal(expected, FileManager.GetCleanFileFolder(value, -1));
        }

        [Fact]
        internal void ConcatPaths_ReturnsPath()
        {
            var paths = new[]
            {
                @"C:\path",
                "my folder",
                "artist",
                "album",
                null,
                ""
            };

            var expected = @"C:\path\my folder\artist\album";

            Assert.Equal(expected, FileManager.ConcatPaths(paths));
        }

        [Fact]
        internal void GetArtistDirectoryName_NoAPIResult_ReturnsDefaultArtistDirectory()
        {
            var track = new Track
            {
                Title = "Title",
                Artist = "Artist",
                TitleExtended = "Live",
                TitleExtendedSeparatorType = TitleSeparatorType.Dash,
                Album = "Single",
                Ad = false
            };

            Assert.Equal("Artist", FileManager.GetArtistDirectoryName(track, " ", -1));
        }

        [Fact]
        internal void GetArtistDirectoryName_WithAlbumArtistsFromAPI_ReturnsAlbumArtistDirectory()
        {
            var track = new Track
            {
                Title = "Title",
                Artist = "Artist",
                TitleExtended = "Live",
                TitleExtendedSeparatorType = TitleSeparatorType.Dash,
                Album = "Single",
                Ad = false,
                AlbumArtists = new[] { "DJ", "Singer" },
                Performers = new[] { "Lil", "Band" },
            };

            Assert.Equal("DJ, Singer", FileManager.GetArtistDirectoryName(track, " ", -1));
        }

        [Fact]
        internal void GetArtistDirectoryName_WithPerformersFromAPI_ReturnsDefaultArtistAndPerformersDirectory()
        {
            var track = new Track
            {
                Title = "Title",
                Artist = "Artist",
                TitleExtended = "Live",
                TitleExtendedSeparatorType = TitleSeparatorType.Dash,
                Album = "Single",
                Ad = false,
                AlbumArtists = new string[] { },
                Performers = new[] { "Lil'", "Band" },
            };

            Assert.Equal("Artist, Lil', Band", FileManager.GetArtistDirectoryName(track, " ", -1));
        }

        [Fact]
        internal void GetAlbumDirectoryName_WithNoAlbum_ReturnsUntitled()
        {
            var track = new Track
            {
                Title = "Title",
                Artist = "Artist",
                TitleExtended = "Live",
                TitleExtendedSeparatorType = TitleSeparatorType.Dash,
                Album = null,
                Ad = false,
                AlbumArtists = new string[] { },
                Performers = new[] { "Lil", "Band" },
            };

            Assert.Equal("Untitled", FileManager.GetAlbumDirectoryName(track, " ", -1));
        }

        [Fact]
        internal void GetAlbumDirectoryName_WithAlbum_ReturnsAlbum()
        {
            var track = new Track
            {
                Title = "Title",
                Artist = "Artist",
                TitleExtended = "Live",
                TitleExtendedSeparatorType = TitleSeparatorType.Dash,
                Album = "Album",
                Ad = false,
                AlbumArtists = new string[] { },
                Performers = new[] { "Lil", "Band" },
            };

            Assert.Equal("Album", FileManager.GetAlbumDirectoryName(track, " ", -1));
        }

        [Fact]
        internal void GetAlbumDirectoryName_WithAlbumAndYear_ReturnsAlbumRelease()
        {
            var track = new Track
            {
                Title = "Title",
                Artist = "Artist",
                TitleExtended = "Live",
                TitleExtendedSeparatorType = TitleSeparatorType.Dash,
                Album = "Album",
                Year = 2000,
                Ad = false,
                AlbumArtists = new string[] { },
                Performers = new[] { "Lil", "Band" },
            };

            Assert.Equal("Album (2000)", FileManager.GetAlbumDirectoryName(track, " ", -1));
        }

        [Fact]
        internal void GetFolderMaxLength_WithOutputPathTooLong_ReturnsAvailableFileLength()
        {
            var userSettings = new UserSettings
            {
                GroupByFoldersEnabled = false,
                TrackTitleSeparator = " ",
                OutputPath = @"\\network\kali linux\root\home\my user\music\a kind of long directory name to store my recordings on my computer that should be way over the max allowed length for a directory name of 260 caracters on a windows system",
            };

            Assert.True(FileManager.IsOutputPathTooLong(userSettings.OutputPath));

            var (length, pathShape) = FileManager.GetFolderMaxLength(userSettings);

            Assert.Equal($@"{userSettings.OutputPath}\", pathShape);
            Assert.Equal(42, length);
        }

        [Fact]
        internal void GetFolderMaxLength_WithOutputPathTooLong_ReturnsSplitLengthPerFolders()
        {
            var userSettings = new UserSettings
            {
                GroupByFoldersEnabled = true,
                TrackTitleSeparator = " ",
                OutputPath = @"\\network\kali linux\root\home\my user\music\a kind of long directory name to store my recordings on my computer that should be way over the max allowed length for a directory name of 260 caracters on a windows system",
            };

            Assert.True(FileManager.IsOutputPathTooLong(userSettings.OutputPath));

            var (length, pathShape) = FileManager.GetFolderMaxLength(userSettings);

            Assert.Equal($@"{userSettings.OutputPath}\\\", pathShape);
            Assert.Equal(13, length);
        }


        [Fact]
        internal void GetFileMaxLength_TooLongInfoWhenGrouping_ReturnsFileNameLengthLeft()
        {
            var track = new Track
            {
                Title = "Title",
                Artist = "A kind of long artist name that needs to be saved",
                TitleExtended = "Track was recorded in that city during a festival",
                TitleExtendedSeparatorType = TitleSeparatorType.Dash,
                Album = "A kind of long album title that needs to be saved",
                Year = 2000,
                Ad = false,
                AlbumArtists = new string[] { },
                Performers = new[] { "Add this artist too", "Also add this one as well" },
            };

            var userSettings = new UserSettings
            {
                GroupByFoldersEnabled = true,
                TrackTitleSeparator = " ",
                OutputPath = @"\\network\kali linux\root\home\my user\music",
            };

            var (length, pathShape) = FileManager.GetFileMaxLength(track, userSettings);

            Assert.Equal(@"\\network\kali linux\root\home\my user\music\A kind of long artist name that needs to be saved, Add this artist too, Also add this one as well\A kind of long album title that needs to be saved (2000)\", pathShape);
            Assert.Equal(50, length);
        }

        [Fact]
        internal void GetFileMaxLength_TooLongInfoNotGrouping_ReturnsFileNameLengthLeft()
        {
            var track = new Track
            {
                Title = "A kind of long track title that needs to be saved",
                Artist = "A kind of long artist name that needs to be saved",
                TitleExtended = "Track was recorded in that city during a festival",
                TitleExtendedSeparatorType = TitleSeparatorType.Dash,
                Album = "A kind of long album title that needs to be saved",
                Year = 2000,
                Ad = false,
                AlbumArtists = new[] { "Add this artist", "Add this one as well" },
            };

            var userSettings = new UserSettings
            {
                GroupByFoldersEnabled = false,
                TrackTitleSeparator = " ",
                OutputPath = @"\\network\kali linux\root\home\my user\music",
            };

            var (length, pathShape) = FileManager.GetFileMaxLength(track, userSettings);

            Assert.Equal(@"\\network\kali linux\root\home\my user\music\", pathShape);
            Assert.Equal(205, length);
        }
    }
}
