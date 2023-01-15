using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using EspionSpotify.Enums;
using EspionSpotify.Extensions;
using EspionSpotify.Models;
using EspionSpotify.Translations;
using WebUtility = PCLWebUtility.WebUtility;

namespace EspionSpotify.API
{
    public class LastFMAPI : ILastFMAPI, IExternalAPI
    {
        private const string API_URI = "http://ws.audioscrobbler.com/2.0/";
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
            track.Length = trackExtra.Duration.HasValue ? trackExtra.Duration / 1000 : null;
            track.Genres = new string[] { };
            
            var extraLarge = trackExtra.Album?.ExtraLargeCoverUrl;
            var artLargeUrl = trackExtra.Album?.LargeCoverUrl;
            var artMediumUrl = trackExtra.Album?.MediumCoverUrl;
            var artSmallUrl = trackExtra.Album?.SmallCoverUrl;
            var urls = new[] {extraLarge, artLargeUrl, artMediumUrl, artSmallUrl}.Where(i => i != null).ToArray();
            track.AlbumArtUrl = urls.FirstOrDefault(url => new Regex(@"\/300x300\/|\/300s\/").IsMatch(url)) ??
                                urls.FirstOrDefault();

            var extraPerformers = track.ToString().ToPerformers();
            var albumArtists = new[] {track.Artist};
            track.Performers = albumArtists.Concat(extraPerformers).ToArray();
            track.AlbumArtists = albumArtists;
        }

        private string GetTrackInfo(string artist, string title)
        {
            return $"{API_URI}?method=track.getInfo&api_key={_selectedApiKey}&artist={artist}&track={title}";
        }
        
        private string GetAlbumInfo(string artist, string album)
        {
            return $"{API_URI}?method=album.getInfo&api_key={_selectedApiKey}&artist={artist}&album={album}";
        }

        private async Task<LastFMNode> FetchFromAPI(string url)
        {
            var api = new XmlDocument();
            
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

                    return null;
                }
            }
            catch (WebException ex)
            {
                // Silent other Web exception since it may be an issue on the user end.
                Console.WriteLine(ex.Message);
                FrmEspionSpotify.Instance.WriteIntoConsole(I18NKeys.LogException, ex.Message);
                return null;
            }
            catch (XmlException ex)
            {
                // Ignore XML exception since it's out of our control.
                Console.WriteLine(ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Program.ReportException(ex);
                return null;
            }
            
            var apiReturn = api.DocumentElement;

            if (apiReturn == null) return null;
            
            var serializer = new XmlSerializer(typeof(LastFMNode));
            var xmlNode = apiReturn.SelectSingleNode("/lfm");
            if (xmlNode == null) return null;

            var node = serializer.Deserialize(new XmlNodeReader(xmlNode)) as LastFMNode;

            return node?.Status == LastFMNodeStatus.ok ? node : null;
        }
        #region LastFM Track updater

        private async Task<bool> UpdateTrack(Track track, string forceQueryTitle = null)
        {
            var encodedArtist = WebUtility.UrlEncode(track.Artist);
            var encodedTitle = WebUtility.UrlEncode(forceQueryTitle ?? track.Title);

            var url = GetTrackInfo(encodedArtist, encodedTitle);
            var node = await FetchFromAPI(url);

           if (node.Track == null) return false;

            var trackExtra = node.Track;

            await FallbackToSingleAlbumIfNeeded(trackExtra);
            
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

        private async Task FallbackToSingleAlbumIfNeeded(LastFMTrack trackExtra)
        {
            if (trackExtra.Album != null && trackExtra.Album.Artist != "Various Artists") return;
            
            var encodedArtist = WebUtility.UrlEncode(trackExtra.Artist.Name);
            var encodedTitle = WebUtility.UrlEncode(trackExtra.Name);

            var url = GetAlbumInfo(encodedArtist, encodedTitle);
            var node = await FetchFromAPI(url);
            
            if (node.Album == null) return;

            trackExtra.Album ??= new Album();
            trackExtra.Album.FromSingleAlbum(node.Album);
        }

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