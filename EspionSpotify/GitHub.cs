using EspionSpotify.Extensions;
using EspionSpotify.Models;
using EspionSpotify.Models.GitHub;
using EspionSpotify.Properties;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace EspionSpotify
{
    internal static class GitHub
    {
        private const string API_LATEST_RELEASE_URL = "https://api.github.com/repos/jwallet/spy-spotify/releases/latest";

        public const string WEBSITE_FAQ_URL = "https://jwallet.github.io/spy-spotify/faq.html";
        public const string WEBSITE_FAQ_SPOTIFY_API_URL = "https://jwallet.github.io/spy-spotify/faq.html#media-tags-not-found";
        public const string WEBSITE_DONATE_URL = "https://jwallet.github.io/spy-spotify/donate.html";
        public const string REPO_LATEST_RELEASE_URL = "https://github.com/jwallet/spy-spotify/releases/latest";

        public static async Task<Release> GetVersionInformation()
        {
            if (!Uri.TryCreate(API_LATEST_RELEASE_URL, UriKind.Absolute, out var uri)) return null;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = WebRequestMethods.Http.Get;
            request.UserAgent = Constants.SPYTIFY;

            var content = new MemoryStream();
            Release release = null;
            try
            {
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    if (response.StatusCode != HttpStatusCode.OK) return null;

                    using (var reader = response.GetResponseStream())
                    {
                        await reader.CopyToAsync(content);
                    }

                    var body = Encoding.UTF8.GetString(content.ToArray());
                    release = JsonConvert.DeserializeObject<Release>(body);
                }
            }
            catch (Exception ex)
            {
                content.Dispose();
                Console.WriteLine(ex.Message);

            }
            return release;

        }

        public static async Task GetVersion()
        {
            try
            {
                var release = await GetVersionInformation();

                    if (release == null) return;

                    var githubTagVersion = release.tag_name.ToVersion();

                    if (githubTagVersion == null) return;

                    var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

                    if (githubTagVersion <= assemblyVersion) return;
                    if (Settings.Default.app_last_version_prompt.ToVersion() == githubTagVersion) return;

                    var dialogTitle = string.Format(FrmEspionSpotify.Instance.Rm.GetString(I18nKeys.MsgNewVersionTitle), githubTagVersion);
                    var dialogMessage = FrmEspionSpotify.Instance.Rm.GetString(I18nKeys.MsgNewVersionContent);

                    if (!string.IsNullOrEmpty(release.body))
                    {
                        var releaseBodySplitted = release.body.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                        dialogMessage = $"{releaseBodySplitted.Take(4).Aggregate((current, next) => $"{current}\n{next}")}\r\n{dialogMessage}";
                    }


                    var dialogResult = MetroFramework.MetroMessageBox.Show(
                        FrmEspionSpotify.Instance,
                        dialogMessage,
                        dialogTitle,
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.OK)
                    {
                        Update(release.assets_url);
                    }

                    Settings.Default.app_last_version_prompt = githubTagVersion.ToString();
                    Settings.Default.Save();
                }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static async void UserUpdate()
        {
            Release release = await GetVersionInformation();
            Update(release.assets_url);
        }
        public static void Update(string apiurl)
        {
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(apiurl);
            wr.Method = WebRequestMethods.Http.Get;
            wr.UserAgent = Constants.SPYTIFY;
            WebResponse ws = wr.GetResponse();
            string json = new StreamReader(ws.GetResponseStream()).ReadToEnd();
            json = json.Remove(0, 1);
            json = json.Remove(json.Length - 1, 1);
            string downlaodUrl = JObject.Parse(json)["browser_download_url"].ToString();
            WebClient wc = new WebClient();
            wc.DownloadFile(downlaodUrl, "update.zip");
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo();
            string execPath = Assembly.GetEntryAssembly().Location;
            p.StartInfo.FileName = Application.StartupPath + "/Updater/Updater.exe";
            p.StartInfo.Arguments = "\"" + @Application.StartupPath + "/\"";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            p.Start();
            Application.Exit();
        }
    }
}
