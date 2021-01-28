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

        private readonly HttpClient client = new HttpClient();
        private readonly string cid;
        private readonly string cm;
        private readonly string ul;
        private readonly string ua;
        private readonly string cs;
        private readonly string sr;

        public DateTime LastRequest { get; set; } = new DateTime();
        public string LastAction { get; set; } = string.Empty;

        public Analytics(string clientId, string version)
        {
            var osArchitecture = Environment.Is64BitOperatingSystem ? $"Win64; x64;" : "";
            var screenBounderies = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            var osPlatform = $"Windows NT { Environment.OSVersion.Version.Major}.{Environment.OSVersion.Version.Minor}";

            cid = clientId;
            cm = version;
            cs = Environment.OSVersion.ToString();
            ul = CultureInfo.CurrentCulture.Name;
            ua = WebUtility.UrlEncode($"Mozilla/5.0 ({osPlatform}; {osArchitecture}) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.132 Safari/537.36");
            sr = $"{screenBounderies.Width}x{screenBounderies.Height}";
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
                { "cn", Constants.SPYTIFY }, // Campaign name
                { "an", Constants.SPYTIFY }, // App name
                { "cs", WebUtility.UrlEncode(cs)}, // Campaign source, OS Version
                { "ul", ul }, // User Language
                { "sr", sr }, // Screen resolution
                { "ua", ua }, // User-Agent overwrite
                { "dh", "jwallet.github.io/spy-spotify" }, // Document host
                { "dl", $"/{action}" }, // Document link
                { "dt", action }, // Document title
                { "cd", action } // Screen name
            };

            var content = new FormUrlEncodedContent(data);
            var success = false;
            
            try
            {
                var resp = await client.PostAsync(ANALYTICS_URL, content);
                success = resp.IsSuccessStatusCode;
            } catch { }

            LastAction = action;
            LastRequest = DateTime.Now;

            return success;
        }
    }
    #endregion analytics
}
