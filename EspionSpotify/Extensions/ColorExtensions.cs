using System.Drawing;

namespace EspionSpotify.Extensions
{
    public static class ColorExtensions
    {
        public static Color SpotifyPrimaryText(this Color _)
        {
            return Color.FromArgb(32, 187, 88);
        }

        public static Color SpotifySecondaryText(this Color _)
        {
            return Color.White;
        }

        public static Color SpotifySecondaryTextAlternate(this Color _)
        {
            return Color.FromArgb(179, 179, 179);
        }

        public static Color SpotifyPrimaryBackground(this Color _)
        {
            return Color.FromArgb(18, 18, 18);
        }

        public static Color SpotifySecondaryBackground(this Color _)
        {
            return Color.FromArgb(24, 24, 24);
        }

        public static Color SpotifySecondaryBackgroundAlternate(this Color _)
        {
            return Color.FromArgb(40, 40, 40);
        }
    }
}