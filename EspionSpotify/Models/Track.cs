using EspionSpotify.MediaTags;
using System.Threading.Tasks;
using TagLib;

namespace EspionSpotify.Models
{
    public class Track
    {
        private const string SPOTIFY = "Spotify";

        private string _artist = null;
        private string _title = null;
        private string _titleExtended = null;

        public string Artist { get => _artist; set => _artist = string.IsNullOrEmpty(value) ? null : value; }
        public string Title { get => _title; set => _title = string.IsNullOrEmpty(value) ? null : value; }
        public bool Ad { get; set; }
        public bool Playing { get; set; }

        public string TitleExtended { get => _titleExtended; set => _titleExtended = string.IsNullOrEmpty(value) ? null : value; }

        public string Album { get; set; }
        public string[] Genres { get; set; }
        public int? AlbumPosition { get; set; }

        public int? CurrentPosition { get; set; }
        public int? Length { get; set; }

        public string[] Performers { get; set; }
        public uint Disc { get; set; }
        public string[] AlbumArtists { get; set; }
        public uint Year { get; set; }

        public string ArtExtraLargeUrl { get; set; }
        public string ArtLargeUrl { get; set; }
        public string ArtMediumUrl { get; set; }
        public string ArtSmallUrl { get; set; }

        public byte[] ArtExtraLarge { get; set; }
        public byte[] ArtLarge { get; set; }
        public byte[] ArtMedium { get; set; }
        public byte[] ArtSmall { get; set; }

        public bool IsNormal() => Artist != null && Title != null && !Ad && Playing;

        public Track() { }

        public Track(Track track)
        {
            Artist = track.Artist;
            Title = track.Title;
            Ad = track.Ad;
            Playing = track.Playing;

            TitleExtended = track.TitleExtended;

            Album = track.Album;
            Genres = track.Genres;
            AlbumPosition = track.AlbumPosition;

            CurrentPosition = track.CurrentPosition;
            Length = track.Length;

            Performers = track.Performers;
            Disc = track.Disc;
            AlbumArtists = track.AlbumArtists;
            Year = track.Year;

            ArtExtraLargeUrl = track.ArtExtraLargeUrl;
            ArtLargeUrl = track.ArtLargeUrl;
            ArtMediumUrl = track.ArtMediumUrl;
            ArtSmallUrl = track.ArtSmallUrl;

            ArtExtraLarge = track.ArtExtraLarge;
            ArtLarge = track.ArtLarge;
            ArtMedium = track.ArtMedium;
            ArtSmall = track.ArtSmall;
        }

        public override string ToString()
        {
            var song = SPOTIFY;

            if (!string.IsNullOrEmpty(Artist) && !string.IsNullOrEmpty(Title))
            {
                song = $"{Artist} - {Title}";

                if (!string.IsNullOrEmpty(TitleExtended))
                {
                    song += $" - {TitleExtended}";
                }
            }

            if (Ad)
            {
                song = Artist;
            }

            return song;
        }

        public string ToTitleString()
        {
            var song = Title ?? "";

            if (!string.IsNullOrEmpty(TitleExtended))
            {
                song += $" - {TitleExtended}";
            }

            return song;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Track)) return false;

            var otherTrack = (Track)obj;
            return otherTrack.Artist == Artist
                && otherTrack.Title == Title
                && otherTrack.TitleExtended == TitleExtended
                && otherTrack.Ad == Ad;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Artist != string.Empty ? Artist.GetHashCode() : 0) * 397)
                    ^ (Title != string.Empty ? Title.GetHashCode() : 0);
            }
        }
    }
}
