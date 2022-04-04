using EspionSpotify.Extensions;
using EspionSpotify.Native;

namespace EspionSpotify.Models
{
    public class OutputFile
    {
        private const int FIRST_SONG_NAME_COUNT = 1;

        private string _file;

        public OutputFile()
        {
            Count = FIRST_SONG_NAME_COUNT;
        }

        public string MediaFile
        {
            get => _file;
            set => _file = Normalize.RemoveDiacritics(value);
        }

        public string BasePath { get; set; }
        public string FoldersPath { get; set; }
        private int Count { get; set; }
        public string Separator { get; set; }
        public string Extension { get; set; }

        internal void Increment()
        {
            Count++;
        }

        public string ToMediaFilePath()
        {
            if (_file.IsNullOrAdOrSpotifyIdleState()) return null;
            return FileManager.ConcatPaths(BasePath, FoldersPath, $"{_file}{GetAddedCount()}.{Extension}");
        }

        public override string ToString()
        {
            return _file.IsNullOrAdOrSpotifyIdleState()
                ? string.Empty
                : FileManager.ConcatPaths("..", FoldersPath, $"{_file}{GetAddedCount()}.{Extension}");
        }

        private string GetAddedCount()
        {
            return Count > FIRST_SONG_NAME_COUNT ? $"{Separator}{Count}" : "";
        }
    }
}