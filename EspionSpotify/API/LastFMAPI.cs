using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using EspionSpotify.Enums;
using EspionSpotify.Models;
using EspionSpotify.Translations;
using WebUtility = PCLWebUtility.WebUtility;

namespace EspionSpotify.API
{
    public class LastFMAPI : ILastFMAPI, IExternalAPI
    {
        private const string API_TRACK_URI = "http://ws.audioscrobbler.com/2.0/?method=track.getInfo";
        private readonly string _selectedApiKey;
        private bool _loggedSilentExceptionOnce;

        public LastFMAPI()
        {
            ApiKeys = new[]
            {
                "c117eb33c9d44d34734dfdcafa7a162d", "01a049d30c4e17c1586707acf5d0fb17",
                "82eb5ead8c6ece5c162b461615495b18"
            };
            var random = new Random();
            _selectedApiKey = ApiKeys[random.Next(ApiKeys.Length)];
        }

        public ExternalAPIType GetTypeAPI => ExternalAPIType.LastFM;

        public async Task<bool> UpdateTrack(Track track)
        {
            return await UpdateTrack(track, null);
        }

        public void Reset()
        {
            _loggedSilentExceptionOnce = false;
        }

        public string[] ApiKeys { get; }

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

        private string GetTrackInfo(string artist, string title)
        {
            return $"{API_TRACK_URI}&api_key={_selectedApiKey}&artist={artist}&track={title}";
        }

        #region LastFM Track updater

        private async Task<bool> UpdateTrack(Track track, string forceQueryTitle = null)
        {
            var api = new XmlDocument();
            var encodedArtist = WebUtility.UrlEncode(track.Artist);
            var encodedTitle = WebUtility.UrlEncode(forceQueryTitle ?? track.Title);

            var url = GetTrackInfo(encodedArtist, encodedTitle);

            try
            {
                api.Load(url);
            }
            catch (WebException ex) when (
                ex.Status == WebExceptionStatus.NameResolutionFailure ||
                ex.Status == WebExceptionStatus.ProxyNameResolutionFailure ||
                ex.Status == WebExceptionStatus.RequestProhibitedByProxy)
            {
                if (!await ApiReload(api, url))
                {
                    Console.WriteLine(ex.Message);
                    if (_loggedSilentExceptionOnce == false)
                    {
                        FrmEspionSpotify.Instance.WriteIntoConsole(I18NKeys.LogException, ex.Message);
                        _loggedSilentExceptionOnce = true;
                    }

                    return false;
                }
            }
            catch (WebException ex)
            {
                // Silent other Web exception since it may be an issue on the user end.
                Console.WriteLine(ex.Message);
                FrmEspionSpotify.Instance.WriteIntoConsole(I18NKeys.LogException, ex.Message);
                return false;
            }
            catch (XmlException ex)
            {
                // Ignore XML exception since it's out of our control.
                Console.WriteLine(ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Program.ReportException(ex);
                return false;
            }

            var apiReturn = api.DocumentElement;

            if (apiReturn == null) return false;

            var serializer = new XmlSerializer(typeof(LastFMNode));
            var xmlNode = apiReturn.SelectSingleNode("/lfm");
            if (xmlNode == null) return false;

            var node = serializer.Deserialize(new XmlNodeReader(xmlNode)) as LastFMNode;

            if (node == null || node.Status != LastFMNodeStatus.ok || node.Track == null) return false;

            var trackExtra = node.Track;

            if (trackExtra?.Album != null)
            {
                MapLastFMTrackToTrack(track, trackExtra);
            }
            else
            {
                var simplifiedTitle = Regex.Replace(track.Title, @" \(.*?\)| \- .*", "");
                if (simplifiedTitle != forceQueryTitle) return await UpdateTrack(track, simplifiedTitle);
            }

            return true;
        }

        #endregion LastFM Track updater

        private async Task<bool> ApiReload(XmlDocument api, string url)
        {
            for (var i = 0; i < 3; i++)
                try
                {
                    api.Load(url);
                    return true;
                }
                catch
                {
                    await Task.Delay(1000);
                }

            return false;
        }

        #region NotImplementedExternalAPI

        public bool IsAuthenticated => true;
        public async Task Authenticate()
        {
            await Task.CompletedTask;
        }
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

        #endregion
    }
}