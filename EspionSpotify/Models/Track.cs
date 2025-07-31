using System.Linq;
using EspionSpotify.Enums;
using EspionSpotify.Extensions;

namespace EspionSpotify.Models
{
    public class Track
    {
        private string _apiArtist;
        private string _apiTitle;
        private string _apiTitleExtended;
        private string _artist;
        private string _title;
        private string _titleExtended;

        public Track()
        {
        }

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

            AlbumArtUrl = track.AlbumArtUrl;
            AlbumArtImage = track.AlbumArtImage;
            Comment = track.Comment;
        }

        public string Artists
        {
            get
            {
                if (AlbumArtists != null && AlbumArtists.Length > 0) return string.Join(", ", AlbumArtists);
                return string.Join(", ", new[] {Artist}.Concat(Performers ?? new string[] { }).Distinct());
            }
        }

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

        public bool Ad { get; set; }
        public bool Playing { get; set; }

        public bool? MetaDataUpdated { get; set; }

        public string Album { get; set; }
        public string[] Genres { get; set; }
        public int? AlbumPosition { get; set; }

        public int? CurrentPosition { get; set; }
        public int? Length { get; set; }

        public string[] Performers { get; set; }
        public int? Disc { get; set; }
        public string[] AlbumArtists { get; set; }
        public int? Year { get; set; }

        public string AlbumArtUrl { get; set; }
        
        public byte[] AlbumArtImage { get; set; }
        
        public string Comment { get; set; }

        private bool IsNormal =>
            !string.IsNullOrEmpty(Artist)
            && !string.IsNullOrEmpty(Title)
            && !Ad;

        public bool IsNormalPlaying => IsNormal && Playing;

        public bool IsUnknown => string.IsNullOrEmpty(Title) && !Artist.IsNullOrAdOrSpotifyIdleState();
        public bool IsUnknownPlaying => IsUnknown && Playing;

        public void SetArtistFromApi(string value)
        {
            if (!string.IsNullOrWhiteSpace(value)) _apiArtist = value;
        }

        public void SetTitleFromApi(string value)
        {
            if (!string.IsNullOrWhiteSpace(value)) _apiTitle = value;
        }

        public void SetTitleExtendedFromApi(string value, TitleSeparatorType separatorType)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _apiTitleExtended = value;
                TitleExtendedSeparatorType = separatorType;
            }
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
                song = $"{Artists} - {Title}";

                if (!string.IsNullOrEmpty(TitleExtended)) song += GetTitleExtended();
            }
            else if (!string.IsNullOrEmpty(Artist))
            {
                song = Artist;
            }

            return song;
        }

        public string ToTitleString()
        {
            var song = IsUnknownPlaying ? Artist : Title;

            if (!string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(TitleExtended)) song += GetTitleExtended();

            return song ?? "";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Track)) return false;

            var otherTrack = (Track) obj;
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