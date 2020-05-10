using EspionSpotify.Models;
using NAudio.Lame;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EspionSpotify.MediaTags
{
    public class MapperID3: IMapperID3
    {
        public int? Count { get; set; }
        public bool OrderNumberInMediaTagEnabled { get; set; }
        public Track Track { get; set; }
        

        public MapperID3(Track track, bool orderNumberInMediaTagEnabled, int? count = null)
        {
            Count = count;
            OrderNumberInMediaTagEnabled = orderNumberInMediaTagEnabled;
            Track = track;
        }

        public async Task<ID3TagData> GetTags()
        {
            var tags = new ID3TagData();

            var trackNumber = GetTrackNumber();
            if (trackNumber.HasValue)
            {
                tags.Track = trackNumber.Value.ToString();
            }

            tags.Title = Track.Title;
            tags.Subtitle = Track.TitleExtended;

            tags.Artist = Track.Performers != null ? string.Join(", ", Track.Performers) : Track.Artist;
            tags.AlbumArtist = Track.Performers != null ? string.Join(", ", Track.AlbumArtists) : Track.Artist;

            tags.Album = Track.Album;
            tags.Year = Track.Year.ToString();
            tags.Genre = Track.Genres != null ? string.Join("/", Track.Genres) : null;

            await FetchMediaPictures();

            tags.AlbumArt = GetMediaPictureTag();

            return tags;
        }

        private async Task FetchMediaPictures()
        {
            if (new[] { Track.ArtExtraLarge, Track.ArtLarge, Track.ArtMedium, Track.ArtSmall }.Any(x => x != null)) return;

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

        private byte[] GetMediaPictureTag()
        {
            var pictures = new List<byte[]>
            {
                Track.ArtExtraLarge,
                Track.ArtLarge,
                Track.ArtMedium,
                Track.ArtSmall
            }.Where(x => x != null);

            return pictures.Any() ? pictures.First() : null;
        }

        private int? GetTrackNumber()
        {
            if (OrderNumberInMediaTagEnabled && Count.HasValue)
            {
                return Count.Value;
            }

            return Track.AlbumPosition;
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
