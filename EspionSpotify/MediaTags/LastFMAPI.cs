using EspionSpotify.Models;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TagLib;

namespace EspionSpotify.MediaTags
{
    public class LastFMAPI: ILastFMAPI
    {
        private const string _apiDomain = "http://ws.audioscrobbler.com/2.0/?method=track.getInfo";
        private readonly string[] _apiKey;
        private readonly Random _random;

        public LastFMTrack TrackInfo { get; set; }
        
        private string ApiUrl(string apiKey, string artist, string title) => $"{_apiDomain }&api_key={apiKey}&artist={artist}&track={title}";

        public LastFMAPI()
        {
            _random = new Random();
            _apiKey = new[] { "c117eb33c9d44d34734dfdcafa7a162d", "01a049d30c4e17c1586707acf5d0fb17", "82eb5ead8c6ece5c162b461615495b18" };
        }

        public LastFMTrack GetTagInfo(Track track)
        {
            TrackInfo = new LastFMTrack();

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
                return null;
            }

            var apiReturn = api.DocumentElement;

            if (apiReturn == null)
            {
                return null;   
            }

            var serializer = new XmlSerializer(typeof(LastFMTrack));
            var node = apiReturn.SelectSingleNode("/lfm/track");
            TrackInfo = serializer.Deserialize(new XmlNodeReader(node)) as LastFMTrack;

            return TrackInfo;
        }
    }
}
