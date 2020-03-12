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

        public string GetTempFile() => _fileSystem.Path.GetTempFileName();

        public OutputFile GetOutputFile(string path)
        {
            var pathName = path + GetFolderPath(_track, _userSettings, _fileSystem);
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
            var pathWithFolder = userSettings.OutputPath + GetFolderPath(track, userSettings, fileSystem);
            var fileName = GenerateFileName(track, userSettings);
            return fileSystem.File.Exists($@"{pathWithFolder}\{fileName}.{GetMediaFormatExtension(userSettings)}");
        }

        public static string GetFolderPath(Track track, UserSettings userSettings, IFileSystem fileSystem)
        {
            string insertArtistDir = null;
            var artistDir = Normalize.RemoveDiacritics(track.Artist);
            artistDir = Regex.Replace(artistDir, _windowsExlcudedChars, string.Empty);

            if (userSettings.GroupByFoldersEnabled)
            {
                insertArtistDir = $@"\{artistDir}";
                fileSystem.Directory.CreateDirectory($@"{userSettings.OutputPath}\{artistDir}");
            }

            return insertArtistDir;
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
            Console.WriteLine(currentFile);
            var folderPath = _fileSystem.Path.GetDirectoryName(currentFile);
            Console.WriteLine(folderPath);
            if (!_fileSystem.Directory.EnumerateFiles(folderPath).Any())
            {
                Console.WriteLine(true);
                _fileSystem.Directory.Delete(folderPath);
            }
        }
    }
}
