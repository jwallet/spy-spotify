using EspionSpotify.MediaTags;
using System.Threading.Tasks;
using TagLib;

namespace EspionSpotify.Models
{
    public class Track
    {
        private const string SPOTIFY = "Spotify";

        public string Artist { get; set; }
        public string Title { get; set; }
        public bool Ad { get; set; }
        public bool Playing { get; set; }

        public string TitleExtended { get; set; }

        public string Album { get; set; }
        public string[] Genres { get; set; }
        public int? AlbumPosition { get; set; }

        public int? CurrentPosition { get; set; }
        public int? Length { get; set; }

        public string[] Performers { get; internal set; }
        public uint Disc { get; internal set; }
        public string[] AlbumArtists { get; internal set; }
        public uint Year { get; internal set; }

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

        public async Task GetArtExtraLargeAsync()
        {
            ArtExtraLarge = await MP3Tags.GetAlbumCover(ArtExtraLargeUrl);
        }
        public async Task GetArtLargeAsync()
        {
            ArtLarge = await MP3Tags.GetAlbumCover(ArtLargeUrl);
        }
        public async Task GetArtMediumAsync()
        {
            ArtMedium = await MP3Tags.GetAlbumCover(ArtMediumUrl);
        }
        public async Task GetArtSmallAsync()
        {
            ArtSmall = await MP3Tags.GetAlbumCover(ArtSmallUrl);
        }

        public override string ToString()
        {
            var song = SPOTIFY;

            if (Artist != null && Title != null)
            {
                song = $"{Artist} - {Title}";

                if (TitleExtended != null)
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

            if (TitleExtended != null)
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
