using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EspionSpotify.Tests
{
    public class TagLibTab : TagLib.Tag
    {
        public TagLibTab() { }

        public override TagLib.TagTypes TagTypes { get; }

        public override uint Track { get; set; }

        public override string Title { get; set; }
        public override string Subtitle { get; set; }
        public override string[] AlbumArtists { get; set; }
        public override string[] Performers { get; set; }

        public override string Album { get; set; }
        public override string[] Genres { get; set; }

        public override uint Year { get; set; }
        public override uint Disc { get; set; }

        public override TagLib.IPicture[] Pictures { get; set; }

        public override void Clear() { }
    }
}
