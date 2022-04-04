using TagLib;

namespace EspionSpotify.Tests
{
    public class TagLibTab : Tag
    {
        public override TagTypes TagTypes { get; }

        public override uint Track { get; set; }

        public override string Title { get; set; }
        public override string Subtitle { get; set; }
        public override string[] AlbumArtists { get; set; }
        public override string[] Performers { get; set; }

        public override string Album { get; set; }
        public override string[] Genres { get; set; }

        public override uint Year { get; set; }
        public override uint Disc { get; set; }

        public override IPicture[] Pictures { get; set; }

        public override void Clear()
        {
        }
    }
}