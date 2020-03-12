using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace EspionSpotify
{
    public class Analytics
    {
        private const string ANALYTICS_URL = "https://www.google-analytics.com/collect";
        private const string ANALYTICS_TID = "UA-125662919-1";

        private readonly HttpClient client = new HttpClient();
        private readonly string cid;
        private readonly string cm;
        private readonly string ul;
        private readonly string cs;

        public DateTime LastRequest { get; set; } = new DateTime();
        public string LastAction { get; set; } = string.Empty;

        public Analytics(string clientId, string version)
        {
            var osVersion = Environment.OSVersion.Version;
            cid = clientId;
            cm = version;
            cs = $"Windows NT {osVersion.Major}.{osVersion.Minor}";
            ul = CultureInfo.CurrentCulture.Name;
        }

        public static string GenerateCID()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task<bool> LogAction(string action)
        {
            if (LastAction.Equals(action) && DateTime.Now - LastRequest < TimeSpan.FromMinutes(5)) return false;

            var data = new Dictionary<string, string>
            {
                { "v", "1" },
                { "tid", ANALYTICS_TID }, // App id
                { "t", "pageview" }, // Analytics type
                { "cid", cid }, // Client id
                { "cm", cm }, // Campaign medium, App version
                { "av", cm }, // App version
                { "cn", "Spytify" }, // Campaign name
                { "an", "Spytify" }, // App name
                { "cs", cs}, // Campaign source, OS Version
                { "ul", ul }, // User Language
                { "ua", "Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)"}, // user-agent overwrite
                { "dh", "jwallet.github.io/spy-spotify" }, // Document host
                { "dl", $"/{action}" }, // Document link
                { "dt", action }, // Document title
                { "cd", action } // Screen name
            };

            var content = new FormUrlEncodedContent(data);
            var resp = await client.PostAsync(ANALYTICS_URL, content);

            LastAction = action;
            LastRequest = DateTime.Now;

            return resp.IsSuccessStatusCode;
        }
    }
}
