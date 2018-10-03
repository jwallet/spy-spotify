using EspionSpotify.Models;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace EspionSpotify
{
    internal class FileManager
    {
        private readonly UserSettings _userSettings;
        private readonly Track _track;

        private const int FirstSongNameCount = 1;
        private readonly string _windowsExlcudedChars = $"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()))}]";

        internal FileManager(UserSettings userSettings, Track track)
        {
            _userSettings = userSettings;
            _track = track;
        }

        internal string GetFileName(string songName, int count, string path = null)
        {
            var ending = _userSettings.MediaFormat.ToString().ToLower();
            songName += count > FirstSongNameCount ? $"{_userSettings.TrackTitleSeparator}{count}" : string.Empty;
            return path != null ? $"{path}\\{songName}.{ending}" : $"{songName}.{ending}";
        }

        internal string BuildFileName(string path, bool includePath = true)
        {
            string songName;
            var track = _userSettings.OrderNumber != -1 && _userSettings.OrderNumberInfrontOfFileEnabled ? $"{_userSettings.OrderNumber:000} " : null;

            if (_userSettings.GroupByFoldersEnabled)
            {
                songName = Normalize.RemoveDiacritics(_track.Title);
                songName = Regex.Replace(songName, _windowsExlcudedChars, string.Empty);
            }
            else
            {
                songName = Normalize.RemoveDiacritics(_track.ToString());
                songName = Regex.Replace(songName, _windowsExlcudedChars, string.Empty);
            }

            var songNameTrackNumber = Regex.Replace($"{track}{songName}", "\\s", _userSettings.TrackTitleSeparator);
            var filename = GetFileName(songNameTrackNumber, FirstSongNameCount, includePath ? path : null);
            var count = FirstSongNameCount;

            while (_userSettings.DuplicateAlreadyRecordedTrack && File.Exists(GetFileName(songNameTrackNumber, count, path)))
            {
                if (includePath) count++;
                filename = GetFileName(songNameTrackNumber, count, includePath ? path : null);
                if (!includePath) count++;
            }

            return filename;
        }

        internal string CreateDirectory()
        {
            string insertArtistDir = null;
            var artistDir = Normalize.RemoveDiacritics(_track.Artist);
            artistDir = Regex.Replace(artistDir, _windowsExlcudedChars, string.Empty);

            if (_userSettings.GroupByFoldersEnabled)
            {
                insertArtistDir = $"//{artistDir}";
                Directory.CreateDirectory($"{_userSettings.OutputPath}//{artistDir}");
            }

            return insertArtistDir;
        }

        internal void DeleteFile(string currentFile)
        {
            File.Delete(currentFile);

            if (_userSettings.GroupByFoldersEnabled)
            {
                DeleteFileFolder(currentFile);
            }
        } 

        private void DeleteFileFolder(string currentFile)
        {
            var folderPath = Path.GetDirectoryName(currentFile);
            if (!Directory.EnumerateFiles(folderPath).Any())
            {
                Directory.Delete(folderPath);
            }
        }
    }
}
