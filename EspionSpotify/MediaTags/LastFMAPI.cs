using EspionSpotify.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using PCLWebUtility;
using EspionSpotify.Spotify;

namespace EspionSpotify.MediaTags
{
    public class LastFMAPI : ILastFMAPI, IExternalAPI
    {
        private const string API_DOMAIN = "http://ws.audioscrobbler.com/2.0/?method=track.getInfo";
        private readonly Random _random;
        private string _selectedApiKey = "";

        public bool IsAuthenticated { get => true; }

        public string[] ApiKeys { get; }

        public LastFMAPI()
        {
            ApiKeys = new[] { "c117eb33c9d44d34734dfdcafa7a162d", "01a049d30c4e17c1586707acf5d0fb17", "82eb5ead8c6ece5c162b461615495b18" };
            _random = new Random();
            _selectedApiKey = ApiKeys[_random.Next(ApiKeys.Length)];
        }

        public async Task Authenticate() { }

        public string GetTrackInfo(string artist, string title) => $"{API_DOMAIN}&api_key={_selectedApiKey}&artist={artist}&track={title}";

        public async Task UpdateTrack(Track track) => await UpdateTrack(track, forceQueryTitle: null);

        private async Task UpdateTrack(Track track, string forceQueryTitle = null)
        {
            var api = new XmlDocument();
            var encodedArtist = WebUtility.UrlEncode(track.Artist);
            var encodedTitle = WebUtility.UrlEncode(forceQueryTitle ?? track.Title);

            try
            {
                var url = GetTrackInfo(encodedArtist, encodedTitle);
                api.Load(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Program.ReportException(ex);
                return;
            }

            var apiReturn = api.DocumentElement;

            if (apiReturn == null) return;

            var serializer = new XmlSerializer(typeof(LastFMNode));
            var xmlNode = apiReturn.SelectSingleNode("/lfm");

            var node = serializer.Deserialize(new XmlNodeReader(xmlNode)) as LastFMNode;

            if (node.Status != Enums.LastFMNodeStatus.ok) return;

            var trackExtra = node.Track;

            if (trackExtra != null && trackExtra.Album != null)
            {
                MapLastFMTrackToTrack(track, trackExtra);
            }
            else
            {
                var simplifiedTitle = Regex.Replace(track.Title, @" \(.*?\)| \- .*", "");
                if (simplifiedTitle != forceQueryTitle)
                {
                    await UpdateTrack(track, simplifiedTitle);
                    return;
                }
            }

            track.MetaDataUpdated = true;
        }

        public void MapLastFMTrackToTrack(Track track, LastFMTrack trackExtra)
        {
            var (titleParts, separatorType) = SpotifyStatus.GetTitleTags(trackExtra.Name, 2);

            track.SetArtistFromAPI(trackExtra.Artist?.Name);
            track.SetTitleFromAPI(SpotifyStatus.GetTitleTag(titleParts, 1));
            track.SetTitleExtendedFromAPI(SpotifyStatus.GetTitleTag(titleParts, 2));
            track.TitleExtendedSeparatorType = separatorType;

            track.Album = trackExtra.Album?.AlbumTitle;
            track.AlbumPosition = trackExtra.Album?.TrackPosition;
            track.Genres = trackExtra.Toptags?.Tag?.Select(x => x?.Name).Where(x => x != null).ToArray();
            track.Length = trackExtra.Duration.HasValue ? trackExtra.Duration / 1000 : null;
            track.ArtExtraLargeUrl = trackExtra.Album?.ExtraLargeCoverUrl;
            track.ArtLargeUrl = trackExtra.Album?.LargeCoverUrl;
            track.ArtMediumUrl = trackExtra.Album?.MediumCoverUrl;
            track.ArtSmallUrl = trackExtra.Album?.SmallCoverUrl;
        }
    }
}
