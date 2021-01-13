using EspionSpotify.Extensions;
using EspionSpotify.Native;
using System;

namespace EspionSpotify.Models
{
    public class OutputFile
    {
        private const int FIRST_SONG_NAME_COUNT = 1;

        private string _file;
        public string File { get => _file; set => _file = Normalize.RemoveDiacritics(value); }

        public string BasePath { get; set; }
        public string FoldersPath { get; set; }
        public int Count { get; private set; }
        public string Separator { get; set; }
        public string Extension { get; set; }

        public OutputFile()
        {
            Count = FIRST_SONG_NAME_COUNT;
        }

        internal void Increment()
        {
            Count++;
        }

        public string ToMediaFilePath()
        {
            if (_file.IsNullOrInvalidSpotifyStatus()) return null;
            return FileManager.ConcatPaths(BasePath, FoldersPath, $"{_file}{GetAddedCount()}.{Extension}");
        }

        public string ToSpytifyFilePath()
        {
            if (_file.IsNullOrInvalidSpotifyStatus()) return null;
            return FileManager.ConcatPaths(BasePath, FoldersPath, $"{_file}{GetAddedCount()}.{Constants.SPYTIFY.ToLower()}");
        }

        public override string ToString()
        {
            if (_file.IsNullOrInvalidSpotifyStatus()) return null;
            return FileManager.ConcatPaths("..", FoldersPath, $"{_file}{GetAddedCount()}.{Extension}");
        }

        private string GetAddedCount()
        {
            return Count > FIRST_SONG_NAME_COUNT ? $"{Separator}{Count}" : "";
        }
    }
}
