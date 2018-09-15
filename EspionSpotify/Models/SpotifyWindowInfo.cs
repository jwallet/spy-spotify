using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EspionSpotify.Models
{
    public class SpotifyWindowInfo
    {
        public string WindowTitle { get; set; }
        public bool IsPlaying { get; set; }

        public bool IsTitledSpotify { get => WindowTitle?.ToLowerInvariant().Equals("spotify") ?? false; }
    }
}
