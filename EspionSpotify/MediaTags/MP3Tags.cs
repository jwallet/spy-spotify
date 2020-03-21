using EspionSpotify.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
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
        
        private readonly IFileSystem _fileSystem;

        internal MP3Tags() : this(fileSystem: new FileSystem()) { }

        public MP3Tags(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public async Task MapMediaTags(TagLib.Tag tags)
        {
            var trackNumber = GetTrackNumber();
            if (trackNumber.HasValue)
            {
                tags.Track = (uint)trackNumber.Value;
            }

            tags.Title = Track.Title;
            tags.AlbumArtists = Track.AlbumArtists ?? new[] { Track.Artist };
            tags.Performers = Track.Performers ?? new[] { Track.Artist };

            tags.Album = Track.Album;
            tags.Genres = Track.Genres;

            tags.Disc = Track.Disc;
            tags.Year = Track.Year;

            await FetchMediaPictures();

            tags.Pictures = GetMediaPictureTag();
        }

        internal async Task SaveMediaTags()
        {
            var mp3 = TagLib.File.Create(CurrentFile);
            
            await MapMediaTags(mp3.Tag);

            if (_fileSystem.File.Exists(CurrentFile))
            {
                mp3.Save();
            }

            mp3.Dispose();
        }

        private async Task FetchMediaPictures()
        {
            var taskGetArtXL = GetAlbumCover(Track.ArtExtraLargeUrl);
            var taskGetArtL = GetAlbumCover(Track.ArtLargeUrl);
            var taskGetArtM = GetAlbumCover(Track.ArtMediumUrl);
            var taskGetArtS = GetAlbumCover(Track.ArtSmallUrl);

            await Task.WhenAll(taskGetArtXL, taskGetArtL, taskGetArtM, taskGetArtS);

            Track.ArtExtraLarge = await taskGetArtXL;
            Track.ArtLarge = await taskGetArtL;
            Track.ArtMedium = await taskGetArtM;
            Track.ArtSmall = await taskGetArtS;
        }

        private TagLib.IPicture[] GetMediaPictureTag()
        {
            var picture = (new TagLib.IPicture[4]
            {
                GetAlbumCoverToPicture(Track.ArtExtraLarge),
                GetAlbumCoverToPicture(Track.ArtLarge),
                GetAlbumCoverToPicture(Track.ArtMedium),
                GetAlbumCoverToPicture(Track.ArtSmall)
            }).Where(x => x != null).FirstOrDefault();

            return picture == null ? null : new[] { picture };
        }

        private int? GetTrackNumber()
        {
            if (OrderNumberInMediaTagEnabled && Count.HasValue)
            {
                return Count.Value;
            }

            return Track.AlbumPosition;
        }

        private static TagLib.Picture GetAlbumCoverToPicture(byte[] data)
        {
            if (data == null) return null;

            return new TagLib.Picture
            {
                Type = TagLib.PictureType.FrontCover,
                MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg,
                Data = data
            };
        }

        private static async Task<byte[]> GetAlbumCover(string link)
        {
            if (string.IsNullOrWhiteSpace(link)) return null;

            try
            {
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
            catch
            {
                return null;
            }
        }
    }
}
