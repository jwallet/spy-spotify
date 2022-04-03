using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EspionSpotify.Extensions;
using EspionSpotify.Models.GitHub;
using EspionSpotify.Properties;
using EspionSpotify.Translations;
using MetroFramework;
using Newtonsoft.Json;

namespace EspionSpotify
{
    internal static class GitHub
    {
        private const string API_LATEST_RELEASE_URL =
            "https://api.github.com/repos/jwallet/spy-spotify/releases/latest";

        public const string WEBSITE_FAQ_URL = "https://jwallet.github.io/spy-spotify/faq.html";

        public const string WEBSITE_FAQ_SPOTIFY_API_URL =
            "https://jwallet.github.io/spy-spotify/faq.html#media-tags-not-found";

        public const string WEBSITE_DONATE_URL = "https://jwallet.github.io/spy-spotify/donate.html";
        // public const string REPO_LATEST_RELEASE_URL = "https://github.com/jwallet/spy-spotify/releases/latest";

        public static async Task GetVersion()
        {
            if (!Uri.TryCreate(API_LATEST_RELEASE_URL, UriKind.Absolute, out var uri)) return;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var request = (HttpWebRequest) WebRequest.Create(uri);
            request.Method = WebRequestMethods.Http.Get;
            request.UserAgent = Constants.SPYTIFY;

            var content = new MemoryStream();

            try
            {
                using (var response = (HttpWebResponse) await request.GetResponseAsync())
                {
                    if (response.StatusCode != HttpStatusCode.OK) return;

                    using (var reader = response.GetResponseStream())
                    {
                        if (reader != null) await reader.CopyToAsync(content);
                    }

                    var body = Encoding.UTF8.GetString(content.ToArray());
                    var release = JsonConvert.DeserializeObject<Release>(body);

                    if (release == null || release.prerelease || release.draft) return;

                    var githubTagVersion = release.tag_name.ToVersion();
                    var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

                    if (githubTagVersion == null || githubTagVersion <= assemblyVersion) return;
                    if (Settings.Default.app_last_version_prompt.ToVersion() == githubTagVersion) return;

                    var dialogTitle = string.Format(FrmEspionSpotify.Instance.Rm.GetString(I18NKeys.MsgNewVersionTitle),
                        githubTagVersion);
                    var dialogMessage = FrmEspionSpotify.Instance.Rm.GetString(I18NKeys.MsgNewVersionContent);

                    if (!string.IsNullOrEmpty(release.body))
                    {
                        var releaseBodySplit =
                            release.body.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);
                        dialogMessage =
                            $"{releaseBodySplit.TakeWhile(x => x.StartsWith("- ")).Take(5).Aggregate((current, next) => $"{current}\n{next}")}\r\n\r\n{dialogMessage}";
                    }

                    var dialogResult = MetroMessageBox.Show(
                        FrmEspionSpotify.Instance,
                        dialogMessage,
                        dialogTitle,
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Question,
                        260);

                    if (dialogResult == DialogResult.OK) Update();

                    Settings.Default.app_last_version_prompt = githubTagVersion.ToString();
                    Settings.Default.Save();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void Update()
        {
            Process.Start(new ProcessStartInfo(Application.StartupPath + "/Updater/Updater.exe"));
            Application.Exit();
        }
    }
}