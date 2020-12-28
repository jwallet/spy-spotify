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

        private static readonly string _windowsExlcudedChars = $"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()))}]";

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
            var folderPath = GetFolderPath(_track, _userSettings);
            var pathName = _userSettings.OutputPath + folderPath;

            CreateDirectories(_track, _userSettings);

            var fileName = GenerateFileName(_track, _userSettings, _now);
            var extension = GetMediaFormatExtension(_userSettings);

            var outputFile = new OutputFile
            {
                Path = pathName,
                File = fileName,
                Separator = _userSettings.TrackTitleSeparator,
                Extension = extension
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
            try
            {
                if (_fileSystem.File.Exists(currentFile))
                {
                    _fileSystem.File.Delete(currentFile);
                }

                if (_userSettings.GroupByFoldersEnabled && Path.GetExtension(currentFile).ToLowerInvariant().Contains(GetMediaFormatExtension(_userSettings)))
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
            return fileSystem.File.Exists($@"{pathWithFolder}\{fileName}.{GetMediaFormatExtension(userSettings)}");
        }

        public static string GetFolderPath(Track track, UserSettings userSettings)
        {
            if (!userSettings.GroupByFoldersEnabled) return null;

            var artistDir = GetArtistFolderPath(track, userSettings.TrackTitleSeparator);
            var albumDir = GetAlbumFolderPath(track, userSettings.TrackTitleSeparator);

            return $@"\{artistDir}\{albumDir}";
        }

        private static string GetArtistFolderPath(Track track, string trackTitleSeparator)
        {
            var artistDir = Normalize.RemoveDiacritics(track.Artist);
            return Regex.Replace(artistDir, _windowsExlcudedChars, string.Empty).Replace(" ", trackTitleSeparator);
        }

        private static string GetAlbumFolderPath(Track track, string trackTitleSeparator)
        {
            var albumInfos = new List<string>() { string.IsNullOrEmpty(track.Album) ? Constants.UNTITLED_ALBUM : Normalize.RemoveDiacritics(track.Album) };
            if (track.Year.HasValue) albumInfos.Add($"({track.Year.Value})");
            return Regex.Replace(string.Join(" ", albumInfos), _windowsExlcudedChars, string.Empty).Replace(" ", trackTitleSeparator);
        }

        private void CreateDirectories(Track track, UserSettings userSettings)
        {
            if (!userSettings.GroupByFoldersEnabled) return;

            var artistDir = GetArtistFolderPath(track, userSettings.TrackTitleSeparator);
            var albumDir = GetAlbumFolderPath(track, userSettings.TrackTitleSeparator);

            var outputPath = userSettings.OutputPath;
            CreateDirectory($@"{outputPath}\{artistDir}");
            CreateDirectory($@"{outputPath}\{artistDir}\{albumDir}");
        }

        private void CreateDirectory(string path)
        {
            if (path.Split(new[] { '\\' }).Contains("")) return;
            if (_fileSystem.Directory.Exists(path)) return;
            _fileSystem.Directory.CreateDirectory(path);
        }

        private static string GetMediaFormatExtension(UserSettings userSettings)
        {
            return userSettings.MediaFormat.ToString().ToLower();
        }

        private static string GenerateFileName(Track track, UserSettings userSettings, DateTime now)
        {
            var fileName = userSettings.GroupByFoldersEnabled
                ? Normalize.RemoveDiacritics(track.ToTitleString())
                : Normalize.RemoveDiacritics(track.ToString());

            if (track.Ad && !track.IsUnknown)
            {
                fileName = $"{Constants.ADVERTISEMENT} {now.ToString("yyyyMMddHHmmss")}";
            }

            fileName = Regex.Replace(fileName, _windowsExlcudedChars, string.Empty);

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
