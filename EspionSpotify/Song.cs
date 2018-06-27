using System;
using System.Threading.Tasks;
using SpotifyAPI.Local.Enums;
using TagLib;
using Track = SpotifyAPI.Local.Models.Track;

namespace EspionSpotify
{
    internal class Song
    {
        public readonly string Artist;
        public readonly string Album;
        public readonly string Title;

        public readonly int? Length;
        public double CurrentLength;
        public readonly string Type;
        public readonly bool IsAd;
        public readonly bool IsOther;
        public readonly bool IsNormal;

        public byte[] ArtLarge;
        public byte[] ArtExtraLarge;

        public Song()
        {
            Album = null;
            Artist = null;
            Title = null;
            IsAd = false;
            IsOther = false;
            IsNormal = false;
        }

        public Song(Track track)
        {
            Album = track?.AlbumResource?.Name;
            Artist = track?.ArtistResource?.Name;
            Title = track?.TrackResource?.Name;

            Type = track?.TrackType;
            Length = track?.Length;
            IsAd = (track?.IsAd() ?? false) && !(track?.TrackResource?.Uri != null && Type == "normal");
            IsOther = track?.IsOtherTrackType() ?? false;
            IsNormal = Title != null && Artist != null;

            if (IsAd || IsOther) return;

            Task.Run(() => GetPictures(track));
        }

        private async void GetPictures(Track track)
        {
            try
            {
                ArtLarge = await track?.GetAlbumArtAsByteArrayAsync(AlbumArtSize.Size160);
            }
            catch (Exception ex)
            {
                ArtLarge = null;
                Console.WriteLine(ex.Message);
            }

            try
            {
                ArtExtraLarge = await track?.GetAlbumArtAsByteArrayAsync(AlbumArtSize.Size320);
            }
            catch (Exception ex)
            {
                ArtExtraLarge = null;
                Console.WriteLine(ex.Message);
            }
        }

        public override string ToString()
        {
            if (Artist != null && Title != null) return $"{Artist} - {Title}";

            const string spotify = "Spotify";
            if (IsAd)
            {
                return $"{spotify} - {FrmEspionSpotify.Rm.GetString($"logAd")}";
            }

            return IsOther ? $"{spotify} - {FrmEspionSpotify.Rm.GetString($"logOther")}" : spotify;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Song)) return false;

            var otherSong = (Song) obj;
            return otherSong.Artist == Artist
                && otherSong.Title == Title
                && otherSong.Length == Length
                && otherSong.Album == Album
                && otherSong.Type == Type
                && otherSong.IsAd == IsAd
                && otherSong.IsOther == IsOther;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Artist != "" ? Artist.GetHashCode() : 0) * 397) ^ (Title != "" ? Title.GetHashCode() : 0);
            }
        }

        private static Picture StreamReaderForBitmap(byte[] bytes)
        {
            
            if (bytes == null) return null;
            var pic = new Picture
            {
                MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg,
                Type = PictureType.FrontCover,
                Description = "Cover",
                Data = bytes
            };
            return pic;
        }

        public Picture GetArtExtraLarge()
        {
            return StreamReaderForBitmap(ArtExtraLarge);
        }

        public Picture GetArtLarge()
        {
            return StreamReaderForBitmap(ArtLarge);
        }
    }
}
