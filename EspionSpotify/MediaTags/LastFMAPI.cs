using EspionSpotify.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace EspionSpotify.MediaTags
{
    public class LastFMAPI: IExternalAPI
    {
        private const string API_DOMAIN = "http://ws.audioscrobbler.com/2.0/?method=track.getInfo";
        private readonly string[] _apiKey;
        private readonly Random _random;

        private string ApiUrl(string apiKey, string artist, string title) => $"{API_DOMAIN }&api_key={apiKey}&artist={artist}&track={title}";

        public LastFMAPI()
        {
            _random = new Random();
            _apiKey = new[] { "c117eb33c9d44d34734dfdcafa7a162d", "01a049d30c4e17c1586707acf5d0fb17", "82eb5ead8c6ece5c162b461615495b18" };
        }

        public async Task<bool> UpdateTrack(Track track)
        {
            var api = new XmlDocument();
            var artist = PCLWebUtility.WebUtility.UrlEncode(track.Artist);
            var title = PCLWebUtility.WebUtility.UrlEncode(track.Title);
            var apiKey = _apiKey[_random.Next(3)];

            try
            {
                api.Load(ApiUrl(apiKey, artist, title));
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

            var serializer = new XmlSerializer(typeof(LastFMTrack));
            var node = apiReturn.SelectSingleNode("/lfm/track");

            var trackExtra = serializer.Deserialize(new XmlNodeReader(node)) as LastFMTrack;

            if (trackExtra != null && trackExtra.Album != null)
            {
                track = MapLastFMTrackToTrack(track, trackExtra);
            }
            else
            {
                var retryWithTrack = track;
                retryWithTrack.Title = Regex.Replace(retryWithTrack.Title, @" \(.*?\)| \- .*", "");
                if (await UpdateTrack(retryWithTrack))
                {
                    track = MapLastFMTrackToTrack(retryWithTrack, trackExtra);
                }
            }

            return true;
        }

        private Track MapLastFMTrackToTrack(Track track, LastFMTrack trackExtra)
        {
            return new Track(track)
            {
                Album = trackExtra.Album?.AlbumTitle,
                AlbumPosition = trackExtra.Album?.TrackPosition,
                Genres = trackExtra.Toptags?.Tag?.Select(x => x.Name).ToArray(),
                Length = trackExtra.Duration / 1000,
                ArtExtraLargeUrl = trackExtra.Album?.ExtraLargeCoverUrl,
                ArtLargeUrl = trackExtra.Album?.LargeCoverUrl,
                ArtMediumUrl = trackExtra.Album?.MediumCoverUrl,
                ArtSmallUrl = trackExtra.Album?.SmallCoverUrl
            };
        }
    }
}
