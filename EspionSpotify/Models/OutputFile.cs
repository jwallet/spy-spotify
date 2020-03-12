using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EspionSpotify.Models
{
    public class OutputFile
    {
        private const string SPYTIFY = "spytify";
        private const int FIRST_SONG_NAME_COUNT = 1;

        public string Path { get; set; }
        public string File { get; set; }
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
            return $@"{Path}\{File}{GetAddedCount()}.{Extension}";
        }

        public string ToPendingFileString()
        {
            return $@"{Path}\{File}{GetAddedCount()}.{SPYTIFY}";
        }

        private string GetAddedCount()
        {
            return Count > FIRST_SONG_NAME_COUNT ? $"{Separator}{Count}" : "";
        }
    }
}
