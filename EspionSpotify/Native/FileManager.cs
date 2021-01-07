using EspionSpotify.Extensions;
using EspionSpotify.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.RegularExpressions;

namespace EspionSpotify.Native
{
    public class FileManager
    {
        private readonly UserSettings _userSettings;
        private readonly Track _track;
        private readonly IFileSystem _fileSystem;
        private readonly DateTime _now;

        private static readonly string _windowsExlcudedCharsPattern = $"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()))}]";

        internal FileManager(UserSettings userSettings, Track track, IFileSystem fileSystem) :
            this(userSettings, track, fileSystem, now: DateTime.Now)
        { }

        public FileManager(UserSettings userSettings, Track track, IFileSystem fileSystem, DateTime now)
        {
            _userSettings = userSettings;
            _track = track;
            _fileSystem = fileSystem;
            _now = now;
        }

        internal string GetTempFile() => _fileSystem.Path.GetTempFileName();

        public OutputFile GetOutputFile()
        {
            CreateDirectories(_track, _userSettings);

            var outputFile = new OutputFile
            {
                Path = ConcatPaths(_userSettings.OutputPath, GetFolderPath(_track, _userSettings)),
                File = GenerateFileName(_track, _userSettings, _now),
                Separator = _userSettings.TrackTitleSeparator,
                Extension = GetMediaFormatExtension(_userSettings)
            };

            switch (_userSettings.RecordRecordingsStatus)
            {
                case Enums.RecordRecordingsStatus.Duplicate:
                    while (_fileSystem.File.Exists(outputFile.ToString()))
                    {
                        outputFile.Increment();
                    }
                    return outputFile;
                default:
                    return outputFile;
            }
        }

        public OutputFile UpdateOutputFileWithLatestTrackInfo(OutputFile outputFile, Track track, UserSettings userSettings)
        {
            outputFile.File = GenerateFileName(track, userSettings, _now);
            return outputFile;
        }

        public void DeleteFile(string currentFile)
        {
            if (string.IsNullOrWhiteSpace(currentFile))
            {
                throw new Exception($"File name cannot be null.");
            }
            try
            {
                if (_fileSystem.File.Exists(currentFile))
                {
                    _fileSystem.File.Delete(currentFile);
                }

                if (_userSettings.GroupByFoldersEnabled && Path.GetExtension(currentFile).ToLowerInvariant() != ".tmp")
                {
                    DeleteFileFolder(currentFile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void RenameFile(string source, string destination)
        {
            if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(destination))
            {
                throw new Exception($"Source / Destination file name cannot be null.");
            }
            try
            {
                if (_fileSystem.File.Exists(source))
                {
                    if (_fileSystem.File.Exists(destination))
                    {
                        _fileSystem.File.Delete(destination);
                    }
                    _fileSystem.File.Move(source, destination);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public bool IsPathFileNameExists(Track track, UserSettings userSettings, IFileSystem fileSystem)
        {
            var pathWithFolder = userSettings.OutputPath + GetFolderPath(track, userSettings);
            var fileName = GenerateFileName(track, userSettings, _now);
            var filePath = ConcatPaths(pathWithFolder, $"{fileName}.{GetMediaFormatExtension(userSettings)}");
            return fileSystem.File.Exists(filePath);
        }

        public static string GetFolderPath(Track track, UserSettings userSettings)
        {
            if (!userSettings.GroupByFoldersEnabled) return null;

            var artistDir = GetArtistDirectoryName(track, userSettings.TrackTitleSeparator);
            var albumDir = GetAlbumDirectoryName(track, userSettings.TrackTitleSeparator);

            if (string.IsNullOrEmpty(artistDir) || string.IsNullOrEmpty(albumDir)) throw new Exception("Artist / Album cannot be null.");

            return ConcatPaths(artistDir, albumDir);
        }

        public static string GetCleanPath(string path)
        {
            return Regex.Replace(path.TrimEndPath(), _windowsExlcudedCharsPattern, string.Empty);
        }

        public static string ConcatPaths(params string[] paths)
        {
            return string.Join(@"\", paths.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        private static string GetArtistDirectoryName(Track track, string trackTitleSeparator)
        {
            var artistDir = Normalize.RemoveDiacritics(track.Artist);
            return GetCleanPath(artistDir).Replace(" ", trackTitleSeparator);
        }

        private static string GetAlbumDirectoryName(Track track, string trackTitleSeparator)
        {
            var albumInfos = new List<string>() { string.IsNullOrEmpty(track.Album) ? Constants.UNTITLED_ALBUM : Normalize.RemoveDiacritics(track.Album) };
            if (track.Year.HasValue) albumInfos.Add($"({track.Year.Value})");
            return GetCleanPath(string.Join(" ", albumInfos)).Replace(" ", trackTitleSeparator);
        }

        private void CreateDirectories(Track track, UserSettings userSettings)
        {
            if (!userSettings.GroupByFoldersEnabled) return;

            var artistDir = GetArtistDirectoryName(track, userSettings.TrackTitleSeparator);
            var albumDir = GetAlbumDirectoryName(track, userSettings.TrackTitleSeparator);

            CreateDirectory(userSettings.OutputPath, artistDir);
            CreateDirectory(userSettings.OutputPath, artistDir, albumDir);
        }

        private void CreateDirectory(string outputPath, params string[] directories)
        {
            if (directories.Any(x => string.IsNullOrWhiteSpace(x)))
            {
                throw new Exception("One or more directories has no name.");
            }

            var path = ConcatPaths(new[] { outputPath }.Concat(directories).ToArray());
            if (_fileSystem.Directory.Exists(path)) return;
            _fileSystem.Directory.CreateDirectory(path);
        }

        private static string GetMediaFormatExtension(UserSettings userSettings)
        {
            return userSettings.MediaFormat.ToString().ToLower();
        }

        private static string GenerateFileName(Track track, UserSettings userSettings, DateTime now)
        {
            if (string.IsNullOrEmpty(track.Artist)) throw new Exception("Cannot recognize this type of track.");
            
            var fileName = Normalize.RemoveDiacritics(userSettings.GroupByFoldersEnabled
                ? track.ToTitleString()
                : track.ToString());

            if (track.Ad && !track.IsUnknown)
            {
                fileName = $"{Constants.ADVERTISEMENT} {now.ToString("yyyyMMddHHmmss")}";
            }
            
            fileName = GetCleanPath(fileName);

            if (string.IsNullOrWhiteSpace(fileName)) throw new Exception("File name cannot be empty.");
            if (new[] { Constants.SPOTIFY, Constants.SPOTIFYFREE }.Contains(fileName)) throw new Exception($"File name cannot be {Constants.SPOTIFY}.");

            var trackNumber = userSettings.OrderNumberInfrontOfFileEnabled ? (userSettings.OrderNumberAsFile ?? 0).ToString($"{userSettings.OrderNumberMask} ") : null;
            return Regex.Replace($"{trackNumber}{fileName}", @"\s", userSettings.TrackTitleSeparator); ;
        }

        private void DeleteFileFolder(string currentFile)
        {
            var folderPath = _fileSystem.Path.GetDirectoryName(currentFile);
            if (!_fileSystem.Directory.EnumerateFiles(folderPath).Any())
            {
                try { _fileSystem.Directory.Delete(folderPath); }
                catch (Exception ex) { Console.WriteLine(ex.Message); }

                var subFolderPath = _fileSystem.Path.GetDirectoryName(folderPath);
                if (_userSettings.OutputPath != subFolderPath && subFolderPath.Contains(_userSettings.OutputPath))
                {
                    DeleteFileFolder(folderPath);
                }
            }
        }
    }
}
