using EspionSpotify.Models;
using EspionSpotify.Spotify;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.RegularExpressions;

namespace EspionSpotify
{
    public class FileManager
    {
        private readonly UserSettings _userSettings;
        private readonly Track _track;
        private readonly IFileSystem _fileSystem;

        private static readonly string _windowsExlcudedChars = $"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()))}]";

        public FileManager(UserSettings userSettings, Track track, IFileSystem fileSystem)
        {
            _userSettings = userSettings;
            _track = track;
            _fileSystem = fileSystem;
        }

        internal string GetTempFile() => _fileSystem.Path.GetTempFileName();

        public OutputFile GetOutputFile()
        {
            var folderPath = GetFolderPath(_track, _userSettings);
            var pathName = _userSettings.OutputPath + folderPath;
            CreateDirectories(_userSettings);

            var fileName = GenerateFileName(_track, _userSettings);
            var extension = GetMediaFormatExtension(_userSettings);

            var outputFile = new OutputFile
            {
                Path = pathName,
                File = fileName,
                Separator = _userSettings.TrackTitleSeparator,
                Extension = extension
            };

            while (_userSettings.DuplicateAlreadyRecordedTrack && _fileSystem.File.Exists(outputFile.ToString()))
            {
                outputFile.Increment();
            }

            return outputFile;
        }


        public void DeleteFile(string currentFile)
        {
            if (_fileSystem.File.Exists(currentFile))
            {
                _fileSystem.File.Delete(currentFile);
            }

            if (_userSettings.GroupByFoldersEnabled)
            {
                DeleteFileFolder(currentFile);
            }
        }

        public void Rename(string source, string destination)
        {
            if (_fileSystem.File.Exists(source))
            {
                _fileSystem.File.Move(source, destination);
            }
        }

        public static bool IsPathFileNameExists(Track track, UserSettings userSettings, IFileSystem fileSystem)
        {
            var pathWithFolder = userSettings.OutputPath + GetFolderPath(track, userSettings);
            var fileName = GenerateFileName(track, userSettings);
            return fileSystem.File.Exists($@"{pathWithFolder}\{fileName}.{GetMediaFormatExtension(userSettings)}");
        }

        public static string GetFolderPath(Track track, UserSettings userSettings)
        {
            if (!userSettings.GroupByFoldersEnabled) return null;

            var artistDir = GetArtistFolderPath(track.Artist);
            var albumDir = GetAlbumFolderPath(track.Album);

            return $@"\{artistDir}\{albumDir}";
        }

        private static string GetArtistFolderPath(string trackArtist)
        {
            var artistDir = Normalize.RemoveDiacritics(trackArtist);
            return Regex.Replace(artistDir, _windowsExlcudedChars, string.Empty);
        }

        private static string GetAlbumFolderPath(string trackAlbum)
        {
            var albumDir = string.IsNullOrEmpty(trackAlbum) ? Track.UNTITLED_ALBUM : Normalize.RemoveDiacritics(trackAlbum);
            return Regex.Replace(albumDir, _windowsExlcudedChars, string.Empty);
        }

        private void CreateDirectories(UserSettings userSettings)
        {
            if (!userSettings.GroupByFoldersEnabled) return;

            var artistDir = GetArtistFolderPath(_track.Artist);
            var albumDir = GetAlbumFolderPath(_track.Album);
            CreateDirectory($@"{_userSettings.OutputPath}\{artistDir}");
            CreateDirectory($@"{_userSettings.OutputPath}\{artistDir}\{albumDir}");
        }

        private void CreateDirectory(string path)
        {
            if (_fileSystem.Directory.Exists(path)) return;
            _fileSystem.Directory.CreateDirectory(path);
        }

        private static string GetMediaFormatExtension(UserSettings userSettings)
        {
            return userSettings.MediaFormat.ToString().ToLower();
        }

        private static string GenerateFileName(Track track, UserSettings userSettings)
        {
            string fileName;

            if (userSettings.GroupByFoldersEnabled)
            {
                fileName = Normalize.RemoveDiacritics(track.ToTitleString());
                fileName = Regex.Replace(fileName, _windowsExlcudedChars, string.Empty);
            }
            else
            {
                fileName = Normalize.RemoveDiacritics(track.ToString());
                fileName = Regex.Replace(fileName, _windowsExlcudedChars, string.Empty);
            }

            var trackNumber = userSettings.OrderNumber?.ToString("000 ") ?? null;
            return Regex.Replace($"{trackNumber}{fileName}", @"\s", userSettings.TrackTitleSeparator); ;
        }

        private void DeleteFileFolder(string currentFile)
        {
            var folderPath = _fileSystem.Path.GetDirectoryName(currentFile);
            if (!_fileSystem.Directory.EnumerateFiles(folderPath).Any())
            {
                try { _fileSystem.Directory.Delete(folderPath); }
                catch { }

                var subFolderPath = _fileSystem.Path.GetDirectoryName(folderPath);
                if (_userSettings.OutputPath != subFolderPath && subFolderPath.Contains(_userSettings.OutputPath))
                {
                    DeleteFileFolder(folderPath);
                }
            }
        }
    }
}
