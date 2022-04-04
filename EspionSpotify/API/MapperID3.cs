using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using EspionSpotify.Enums;
using EspionSpotify.Models;
using TagLib;
using File = TagLib.File;
using Tag = TagLib.Tag;

namespace EspionSpotify.API
{
    public class MapperID3
    {
        private readonly bool _extraTitleToSubtitleEnabled;

        private readonly IFileSystem _fileSystem;

        internal MapperID3(string currentFile, Track track, UserSettings userSettings) :
            this(new FileSystem(), currentFile, track, userSettings)
        {
        }

        public MapperID3(IFileSystem fileSystem, string currentFile, Track track, UserSettings userSettings)
        {
            _fileSystem = fileSystem;
            CurrentFile = currentFile;
            Track = track;
            OrderNumberInMediaTagEnabled = userSettings.OrderNumberInMediaTagEnabled;
            Count = userSettings.OrderNumberAsTag;
            _extraTitleToSubtitleEnabled = userSettings.ExtraTitleToSubtitleEnabled;
        }

        private string CurrentFile { get; }
        private int? Count { get; }
        private bool OrderNumberInMediaTagEnabled { get; }
        public Track Track { get; }


        private bool IsMovingExtraTitleToSubtitle
        {
            get
            {
                var separatorType = Track.TitleExtendedSeparatorType;
                return separatorType == TitleSeparatorType.Dash
                       || _extraTitleToSubtitleEnabled && separatorType == TitleSeparatorType.Parenthesis;
            }
        }

        public async Task MapTags(Tag tags)
        {
            var trackNumber = GetTrackNumber();
            if (trackNumber.HasValue) tags.Track = (uint) trackNumber.Value;

            tags.Title = IsMovingExtraTitleToSubtitle ? Track.Title : Track.ToTitleString();
            tags.Subtitle = IsMovingExtraTitleToSubtitle ? Track.TitleExtended : null;

            tags.AlbumArtists = Track.AlbumArtists ?? new[] {Track.Artist};
            tags.Performers = Track.Performers ?? new[] {Track.Artist};

            tags.Album = Track.Album;
            tags.Genres = Track.Genres;

            tags.Disc = (uint) (Track.Disc ?? 0);
            tags.Year = (uint) (Track.Year ?? 0);

            await FetchMediaPictures();

            tags.Pictures = GetMediaPictureTag();
        }

        #region MP3 Tags updater

        internal async Task SaveMediaTags()
        {
            await Task.Delay(1000);
            using (var mp3 = File.Create(CurrentFile))
            {
                await MapTags(mp3.Tag);

                if (_fileSystem.File.Exists(CurrentFile)) mp3.Save();
            }
        }

        #endregion MP3 Tags updater

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

        private IPicture[] GetMediaPictureTag()
        {
            var pictures = new IPicture[4]
            {
                GetAlbumCoverToPicture(Track.ArtExtraLarge),
                GetAlbumCoverToPicture(Track.ArtLarge),
                GetAlbumCoverToPicture(Track.ArtMedium),
                GetAlbumCoverToPicture(Track.ArtSmall)
            }.Where(x => x != null).ToList();

            return pictures.Any() ? new[] {pictures.First()} : null;
        }

        private int? GetTrackNumber()
        {
            if (OrderNumberInMediaTagEnabled && Count.HasValue) return Count.Value;

            return Track.AlbumPosition;
        }

        private static Picture GetAlbumCoverToPicture(byte[] data)
        {
            if (data == null) return null;

            return new Picture
            {
                Type = PictureType.FrontCover,
                MimeType = MediaTypeNames.Image.Jpeg,
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
                    if (stream == null) return null;
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