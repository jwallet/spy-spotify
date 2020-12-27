using EspionSpotify.Enums;
using EspionSpotify.Extensions;
using EspionSpotify.MediaTags;
using EspionSpotify.Spotify;
using System.Collections.Generic;
using System.Threading.Tasks;
using TagLib;

namespace EspionSpotify.Models
{
    public class Track
    {
        private string _artist = null;
        private string _apiArtist = null;
        private string _title = null;
        private string _apiTitle = null;
        private string _titleExtended = null;
        private string _apiTitleExtended = null;

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

        public TitleSeparatorType TitleExtendedSeparatorType { get; set; } = TitleSeparatorType.None;

        public void SetArtistFromAPI(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _apiArtist = value;
            }
        }
        public void SetTitleFromAPI(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _apiTitle = value;
            }
        }
        public void SetTitleExtendedFromAPI(string value, TitleSeparatorType separatorType)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _apiTitleExtended = value;
                TitleExtendedSeparatorType = separatorType;
            }
        }

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

        public bool IsNormal {
            get =>
                !string.IsNullOrEmpty(Artist)
                && !string.IsNullOrEmpty(Title)
                && !Ad
                && Playing;
        }
        public bool IsUnknown
        {
            get =>
                !string.IsNullOrEmpty(Artist)
                && string.IsNullOrEmpty(Title)
                && !SpotifyStatus.WindowTitleIsAd(Artist)
                && !SpotifyStatus.WindowTitleIsSpotify(Artist)
                && Playing;
        }

        public Track() { }

        public Track(Track track)
        {
            Artist = track.Artist;
            Title = track.Title;
            Ad = track.Ad;
            Playing = track.Playing;

            TitleExtended = track.TitleExtended;
            TitleExtendedSeparatorType = track.TitleExtendedSeparatorType;

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
            if (string.IsNullOrEmpty(TitleExtended)) return null;
            switch (TitleExtendedSeparatorType)
            {
                case TitleSeparatorType.Dash:
                    return $" - {TitleExtended}";
                case TitleSeparatorType.Parenthesis:
                    return $" ({TitleExtended})";
                default:
                    return "";
            }
        }

        public override string ToString()
        {
            var song = Constants.SPOTIFY;

            if (!string.IsNullOrEmpty(Artist) && !string.IsNullOrEmpty(Title))
            {
                var artists = string.Join(", ", AlbumArtists ?? Performers ?? new[] { Artist });
                song = $"{artists} - {Title}";

                if (!string.IsNullOrEmpty(TitleExtended))
                {
                    song += GetTitleExtended();
                }
            }
            else if (!string.IsNullOrEmpty(Artist))
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
