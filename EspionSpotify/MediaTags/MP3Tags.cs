using EspionSpotify.Models;
using System;
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
            var mp3 = TagLib.File.Create(CurrentFile);

            var trackNumber = GetTrackNumber();
            if (trackNumber.HasValue)
            {
                mp3.Tag.Track = (uint)trackNumber.Value;
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

            if (File.Exists(CurrentFile))
            {
                mp3.Save();
            }

            mp3.Dispose();
        }

        private int? GetTrackNumber()
        {
            if (OrderNumberInMediaTagEnabled && Count.HasValue)
            {
                return Count.Value;
            }

            return Track.AlbumPosition;
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
            if (string.IsNullOrWhiteSpace(link)) return null;

            var request = WebRequest.Create(link);
            using (var response = await request.GetResponseAsync())
            {
                var stream = response.GetResponseStream();
                if (stream == null)
                {
                    response.Dispose();
                    return null;
                }
                using (var reader = new BinaryReader(stream))
                {
                    using (var memory = new MemoryStream())
                    {
                        var buffer = reader.ReadBytes(4096);
                        while (buffer.Length > 0)
                        {
                            await memory.WriteAsync(buffer, 0, buffer.Length);
                            buffer = reader.ReadBytes(4096);
                        }
                        response.Dispose();

                        return memory.ToArray();
                    }
                }
            }
        }
    }
}
