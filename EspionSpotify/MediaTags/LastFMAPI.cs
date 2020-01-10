using EspionSpotify.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using PCLWebUtility;

namespace EspionSpotify.MediaTags
{
    public class LastFMAPI: ILastFMAPI, IExternalAPI
    {
        private const string API_DOMAIN = "http://ws.audioscrobbler.com/2.0/?method=track.getInfo";
        private readonly string[] _apiKey;
        private readonly Random _random;

        private string ApiUrl(string apiKey, string artist, string title) => $"{API_DOMAIN}&api_key={apiKey}&artist={artist}&track={title}";

        public LastFMAPI()
        {
            _random = new Random();
            _apiKey = new[] { "c117eb33c9d44d34734dfdcafa7a162d", "01a049d30c4e17c1586707acf5d0fb17", "82eb5ead8c6ece5c162b461615495b18" };
        }

        public async Task<bool> UpdateTrack(Track track)
        {
            var api = new XmlDocument();
            var artist = WebUtility.UrlEncode(track.Artist);
            var title = WebUtility.UrlEncode(track.Title);
            var apiKey = _apiKey[_random.Next(3)];

            try
            {
                var url = ApiUrl(apiKey, artist, title);
                api.Load(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            var apiReturn = api.DocumentElement;

            if (apiReturn == null)
            {
                return false;   
            }

            var serializer = new XmlSerializer(typeof(LastFMNode));
            var xmlNode = apiReturn.SelectSingleNode("/lfm");

            var node = serializer.Deserialize(new XmlNodeReader(xmlNode)) as LastFMNode;

            if (node.Status != Enums.LastFMNodeStatus.ok) return false;

            var trackExtra = node.Track;

            if (trackExtra != null && trackExtra.Album != null)
            {
                MapLastFMTrackToTrack(track, trackExtra);
            }
            else
            {
                var retryWithTrack = track;
                retryWithTrack.Title = Regex.Replace(retryWithTrack.Title, @" \(.*?\)| \- .*", "");
                if (retryWithTrack.Title != track.Title && await UpdateTrack(retryWithTrack))
                {
                    MapLastFMTrackToTrack(retryWithTrack, trackExtra);
                }
            }

            return true;
        }

        public void MapLastFMTrackToTrack(Track track, LastFMTrack trackExtra)
        {
            track.Album = trackExtra.Album?.AlbumTitle;
            track.AlbumPosition = trackExtra.Album?.TrackPosition;
            track.Genres = trackExtra.Toptags?.Tag?.Select(x => x.Name).ToArray();
            track.Length = trackExtra.Duration / 1000;
            track.ArtExtraLargeUrl = trackExtra.Album?.ExtraLargeCoverUrl;
            track.ArtLargeUrl = trackExtra.Album?.LargeCoverUrl;
            track.ArtMediumUrl = trackExtra.Album?.MediumCoverUrl;
            track.ArtSmallUrl = trackExtra.Album?.SmallCoverUrl;
        }
    }
}
