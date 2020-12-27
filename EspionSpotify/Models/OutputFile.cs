namespace EspionSpotify.Models
{
    public class OutputFile
    {
        private const string SPYTIFY = "spytify";
        private const int FIRST_SONG_NAME_COUNT = 1;

        private string _file;
        public string File { get => _file; set => _file = Normalize.RemoveDiacritics(value); }

        public string Path { get; set; }
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

        public override string ToString()
        {
            return $@"{Path}\{_file}{GetAddedCount()}.{Extension}";
        }

        public string ToPendingFileString()
        {
            return $@"{Path}\{_file}{GetAddedCount()}.{SPYTIFY}";
        }

        private string GetAddedCount()
        {
            return Count > FIRST_SONG_NAME_COUNT ? $"{Separator}{Count}" : "";
        }
    }
}
