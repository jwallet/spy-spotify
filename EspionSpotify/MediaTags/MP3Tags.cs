using EspionSpotify.Models;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace EspionSpotify.MediaTags
{
    public class MP3Tags
    {
        public string CurrentFile { get; set; }
        public int? Count { get; set; }
        public bool OrderNumberInMediaTagEnabled { get; set; }
        public Track Track { get; set; }

        public async Task SaveMediaTags()
        {
            var api = new LastFMAPI();
            var mp3 = TagLib.File.Create(CurrentFile);

            if (OrderNumberInMediaTagEnabled && Count.HasValue)
            {
                mp3.Tag.Track = (uint)Count.Value;
            }
            else if (Track.AlbumPosition != null)
            {
                mp3.Tag.Track = (uint)Track.AlbumPosition;
            }

            mp3.Tag.Title = Track.Title;
            mp3.Tag.AlbumArtists = Track.AlbumArtists ?? new[] { Track.Artist };
            mp3.Tag.Performers = Track.Performers ?? new[] { Track.Artist };
            
            mp3.Tag.Album = Track.Album;
            mp3.Tag.Genres = Track.Genres;

            mp3.Tag.Disc = Track.Disc;
            mp3.Tag.Year = Track.Year;

            if (File.Exists(CurrentFile))
            {
                mp3.Save();
            }

            var taskGetArtXL = Track.GetArtExtraLargeAsync();
            var taskGetArtL = Track.GetArtLargeAsync();
            var taskGetArtM = Track.GetArtMediumAsync();
            var taskGetArtS = Track.GetArtSmallAsync();

            await Task.WhenAll(taskGetArtXL, taskGetArtL, taskGetArtM, taskGetArtS);

            mp3.Tag.Pictures = new TagLib.IPicture[4]
            {
                GetAlbumCoverToPicture(Track.ArtExtraLarge),
                GetAlbumCoverToPicture(Track.ArtLarge),
                GetAlbumCoverToPicture(Track.ArtMedium),
                GetAlbumCoverToPicture(Track.ArtSmall)
            };

            if (File.Exists(CurrentFile)) mp3.Save();

            mp3.Dispose();
        }

        private TagLib.Picture GetAlbumCoverToPicture(byte[] data)
        {
            if (data == null) return new TagLib.Picture();

            return new TagLib.Picture
            {
                Type = TagLib.PictureType.FrontCover,
                MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg,
                Description = "Cover",
                Data = data
            };
        }

        public static async Task<byte[]> GetAlbumCover(string link)
        {
            if (string.IsNullOrEmpty(link)) return null;

            var request = WebRequest.Create(link);
            using (var response = (await request.GetResponseAsync()).GetResponseStream())
            {
                if (response == null) return null;
                using (var reader = new BinaryReader(response))
                {
                    using (var memory = new MemoryStream())
                    {
                        var buffer = reader.ReadBytes(4096);
                        while (buffer.Length > 0)
                        {
                            await memory.WriteAsync(buffer, 0, buffer.Length);
                            buffer = reader.ReadBytes(4096);
                        }
                        return memory.ToArray();
                    }
                }
            }
        }
    }
}
