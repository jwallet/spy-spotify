using PCLWebUtility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace EspionSpotify
{
    #region analytics
    public class Analytics
    {
        private const string ANALYTICS_URL = "https://www.google-analytics.com/collect";
        private const string ANALYTICS_TID = "UA-125662919-1";

        private readonly HttpClient _client = new HttpClient();
        private readonly string _cid;
        private readonly string _cm;
        private readonly string _ul;
        private readonly string _ua;
        private readonly string _cs;
        private readonly string _sr;

        private DateTime LastRequest { get; set; }
        private string LastAction { get; set; } = string.Empty;

        public Analytics(string clientId, string version)
        {
            var osArchitecture = Environment.Is64BitOperatingSystem ? $"Win64; x64;" : "";
            var screenBoundaries = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            var osPlatform = $"Windows NT { Environment.OSVersion.Version.Major}.{Environment.OSVersion.Version.Minor}";

            _cid = clientId;
            _cm = version;
            _cs = Environment.OSVersion.ToString();
            _ul = CultureInfo.CurrentCulture.Name;
            _ua = WebUtility.UrlEncode($"Mozilla/5.0 ({osPlatform}; {osArchitecture}) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.132 Safari/537.36");
            _sr = $"{screenBoundaries.Width}x{screenBoundaries.Height}";
        }

        public static string GenerateCid()
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
                { "cid", _cid }, // Client id
                { "cm", _cm }, // Campaign medium, App version
                { "av", _cm }, // App version
                { "cn", Constants.SPYTIFY }, // Campaign name
                { "an", Constants.SPYTIFY }, // App name
                { "cs", WebUtility.UrlEncode(_cs)}, // Campaign source, OS Version
                { "ul", _ul }, // User Language
                { "sr", _sr }, // Screen resolution
                { "ua", _ua }, // User-Agent overwrite
                { "dh", "jwallet.github.io/spy-spotify" }, // Document host
                { "dl", $"/{action}" }, // Document link
                { "dt", action }, // Document title
                { "cd", action } // Screen name
            };

            var content = new FormUrlEncodedContent(data);
            var success = false;
            
            try
            {
                var resp = await _client.PostAsync(ANALYTICS_URL, content);
                success = resp.IsSuccessStatusCode;
            }
            catch
            {
                // ignored
            }

            LastAction = action;
            LastRequest = DateTime.Now;

            return success;
        }
    }
    #endregion analytics
}
