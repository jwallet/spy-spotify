using EspionSpotify.Models;
using EspionSpotify.Spotify;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace EspionSpotify
{
    public class FileManager
    {
        private readonly UserSettings _userSettings;
        private readonly Track _track;

        private const int FIRST_SONG_NAME_COUNT = 1;
        private const string SPYTIFY = "spytify";
        private static readonly string _windowsExlcudedChars = $"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()))}]";

        public FileManager(UserSettings userSettings, Track track)
        {
            _userSettings = userSettings;
            _track = track;
        }

        public string GetPathName(string fileName, int count, string path = null)
        {
            var fileExtension = GetMediaFormatExtension(_userSettings);
            fileName += count > FIRST_SONG_NAME_COUNT ? $"{_userSettings.TrackTitleSeparator}{count}" : string.Empty;
            var mediaFile = $"{fileName}.{fileExtension}";
            return path != null ? $"{path}\\{mediaFile}" : mediaFile;
        }

        public string BuildFileName(string path)
        {
            var pathWithFolder = path + GetFolderPath(_track, _userSettings);
            var fileName = GetFileName(_track, _userSettings);
            var count = FIRST_SONG_NAME_COUNT;

            var pathName = GetPathName(fileName, count, pathWithFolder);

            while (_userSettings.DuplicateAlreadyRecordedTrack && File.Exists(GetPathName(fileName, count, pathWithFolder)))
            {
                count++;
                pathName = GetPathName(fileName, count, pathWithFolder);
            }

            return pathName;
        }

        public string BuildSpytifyFileName(string fileName)
        {
            return $"{fileName}.{SPYTIFY}";
        }
        
        public string GetFileName(string file)
        {
            return Path.GetFileName(file);
        }

        public void Rename(string source, string destination)
        {
            if (File.Exists(source))
            {
                File.Move(source, destination);
            }
        }

        public void DeleteFile(string currentFile)
        {
            if (File.Exists(currentFile))
            {
                File.Delete(currentFile);
            }

            if (_userSettings.GroupByFoldersEnabled)
            {
                DeleteFileFolder(currentFile);
            }
        }
        private static string GetMediaFormatExtension(UserSettings userSettings)
        {
            return userSettings.MediaFormat.ToString().ToLower();
        }

        public static bool IsPathFileNameExists(Track track, UserSettings userSettings)
        {
            var pathWithFolder = userSettings.OutputPath + GetFolderPath(track, userSettings);
            var fileName = GetFileName(track, userSettings);
            return File.Exists($"{pathWithFolder}\\{fileName}.{GetMediaFormatExtension(userSettings)}");
        }

        public static string GetFolderPath(Track track, UserSettings userSettings)
        {
            string insertArtistDir = null;
            var artistDir = Normalize.RemoveDiacritics(track.Artist);
            artistDir = Regex.Replace(artistDir, _windowsExlcudedChars, string.Empty);

            if (userSettings.GroupByFoldersEnabled)
            {
                insertArtistDir = $"\\{artistDir}";
                Directory.CreateDirectory($"{userSettings.OutputPath}\\{artistDir}");
            }

            return insertArtistDir;
        }

        private static string GetFileName(Track track, UserSettings userSettings)
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
            return Regex.Replace($"{trackNumber}{fileName}", "\\s", userSettings.TrackTitleSeparator); ;
        }

        private static void DeleteFileFolder(string currentFile)
        {
            var folderPath = Path.GetDirectoryName(currentFile);
            if (!Directory.EnumerateFiles(folderPath).Any())
            {
                Directory.Delete(folderPath);
            }
        }
    }
}
