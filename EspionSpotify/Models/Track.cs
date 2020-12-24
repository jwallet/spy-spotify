using EspionSpotify.Enums;
using EspionSpotify.MediaTags;
using System.Collections.Generic;
using System.Threading.Tasks;
using TagLib;

namespace EspionSpotify.Models
{
    public class Track
    {
        private const string SPOTIFY = "Spotify";
        public const string UNTITLED_ALBUM = "Untitled";

        private string _artist = null;
        private string _apiArtist = null;
        private string _title = null;
        private string _apiTitle = null;
        private string _titleExtended = null;
        private string _apiTitleExtended = null;

        private TitleSeparatorType _titleSeparatorType = TitleSeparatorType.Dash;

        public string Artist
        {
            get => _apiArtist ?? _artist;
            set => _artist = string.IsNullOrEmpty(value) ? null : value;
        }
        public string Title
        {
            get => _apiTitle ?? _title;
            set => _title = string.IsNullOrEmpty(value) ? null : value;
        }
        public string TitleExtended
        {
            get => _apiTitleExtended ?? _titleExtended;
            set => _titleExtended = string.IsNullOrEmpty(value) ? null : value;
        }

        public TitleSeparatorType TitleExtendedSeparatorType { set => _titleSeparatorType = value; }

        public void SetArtistFromAPI(string value) => _apiArtist = value;
        public void SetTitleFromAPI(string value) => _apiTitle = value;
        public void SetTitleExtendedFromAPI(string value) => _apiTitleExtended = value;

        public bool Ad { get; set; }
        public bool Playing { get; set; }

        public bool MetaDataUpdated { get; set; }

        public string Album { get; set; }
        public string[] Genres { get; set; }
        public int? AlbumPosition { get; set; }

        public int? CurrentPosition { get; set; }
        public int? Length { get; set; }

        public string[] Performers { get; set; }
        public int? Disc { get; set; }
        public string[] AlbumArtists { get; set; }
        public int? Year { get; set; }

        public string ArtExtraLargeUrl { get; set; }
        public string ArtLargeUrl { get; set; }
        public string ArtMediumUrl { get; set; }
        public string ArtSmallUrl { get; set; }

        public byte[] ArtExtraLarge { get; set; }
        public byte[] ArtLarge { get; set; }
        public byte[] ArtMedium { get; set; }
        public byte[] ArtSmall { get; set; }

        public bool IsNormal { get => !string.IsNullOrEmpty(Artist) && !string.IsNullOrEmpty(Title) && !Ad && Playing; }

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

        private string GetTitleExtended()
        {
            return !string.IsNullOrEmpty(TitleExtended)
                ? _titleSeparatorType == TitleSeparatorType.Dash ? $" - {TitleExtended}" : $" ({TitleExtended})"
                : string.Empty;
        }

        public override string ToString()
        {
            var song = SPOTIFY;

            if (!string.IsNullOrEmpty(Artist) && !string.IsNullOrEmpty(Title))
            {
                var artists = string.Join(",", AlbumArtists ?? Performers ?? new[] { Artist });
                song = $"{artists} - {Title}";

                if (!string.IsNullOrEmpty(TitleExtended))
                {
                    song += GetTitleExtended();
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
                song += GetTitleExtended();
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
