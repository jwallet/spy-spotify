using EspionSpotify.Enums;
using EspionSpotify.Models;
using PCLWebUtility;
using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace EspionSpotify.API
{
    public class LastFMAPI : ILastFMAPI, IExternalAPI
    {
        private const string API_TRACK_URI = "http://ws.audioscrobbler.com/2.0/?method=track.getInfo";
        private readonly Random _random;
        private readonly string _selectedApiKey = "";
        private bool _loggedSilentExceptionOnce = false;

        public string[] ApiKeys { get; }

        public LastFMAPI()
        {
            ApiKeys = new[] { "c117eb33c9d44d34734dfdcafa7a162d", "01a049d30c4e17c1586707acf5d0fb17", "82eb5ead8c6ece5c162b461615495b18" };
            _random = new Random();
            _selectedApiKey = ApiKeys[_random.Next(ApiKeys.Length)];
        }

        public ExternalAPIType GetTypeAPI { get => ExternalAPIType.LastFM; }

        public string GetTrackInfo(string artist, string title) => $"{API_TRACK_URI}&api_key={_selectedApiKey}&artist={artist}&track={title}";

        public async Task UpdateTrack(Track track) => await UpdateTrack(track, forceQueryTitle: null);

        private async Task UpdateTrack(Track track, string forceQueryTitle = null)
        {
            var api = new XmlDocument();
            var encodedArtist = PCLWebUtility.WebUtility.UrlEncode(track.Artist);
            var encodedTitle = PCLWebUtility.WebUtility.UrlEncode(forceQueryTitle ?? track.Title);

            try
            {
                var url = GetTrackInfo(encodedArtist, encodedTitle);
                api.Load(url);
            }
            catch (WebException ex) when (ex.Status == WebExceptionStatus.NameResolutionFailure)
            {
                Console.WriteLine(ex.Message);
                if (_loggedSilentExceptionOnce == false)
                {
                    FrmEspionSpotify.Instance.WriteIntoConsole(I18nKeys.LogException, ex.Message);
                    _loggedSilentExceptionOnce = true;
                }
                return;
            }
            catch (XmlException ex)
            {
                // Ignore XML exception since it's out of our control
                Console.WriteLine(ex.Message);
                return;
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
            track.Album = trackExtra.Album?.AlbumTitle;
            track.AlbumPosition = trackExtra.Album?.TrackPosition;
            track.Genres = trackExtra.Toptags?.Tag?.Select(x => x?.Name).Where(x => x != null).ToArray();
            track.Length = trackExtra.Duration.HasValue ? trackExtra.Duration / 1000 : null;
            track.ArtExtraLargeUrl = trackExtra.Album?.ExtraLargeCoverUrl;
            track.ArtLargeUrl = trackExtra.Album?.LargeCoverUrl;
            track.ArtMediumUrl = trackExtra.Album?.MediumCoverUrl;
            track.ArtSmallUrl = trackExtra.Album?.SmallCoverUrl;
        }

        #region NotImplementedExternalAPI
        public bool IsAuthenticated { get => true; }
        public async Task Authenticate() => await Task.CompletedTask;
        public async Task<(string, bool)> GetCurrentPlayback()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
