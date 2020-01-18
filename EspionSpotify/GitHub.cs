using EspionSpotify.Extensions;
using EspionSpotify.Models.GitHub;
using EspionSpotify.Properties;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using EspionSpotify.Models;
using System.Threading.Tasks;

namespace EspionSpotify
{
    internal static class GitHub
    {
        private const string API_LATEST_RELEASE_URL = "https://api.github.com/repos/jwallet/spy-spotify/releases/latest";
        private const string SPYTIFY = "Spytify";

        public const string REPO_LATEST_RELEASE_URL = "https://github.com/jwallet/spy-spotify/releases/latest";

        public static async Task GetVersion()
        {
            if (!Uri.TryCreate(API_LATEST_RELEASE_URL, UriKind.Absolute, out var uri)) return;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = WebRequestMethods.Http.Get;
            request.UserAgent = SPYTIFY;

            var content = new MemoryStream();

            try
            {
                using (var response = (HttpWebResponse) await request.GetResponseAsync())
                {
                    if (response.StatusCode != HttpStatusCode.OK) return;

                    using (var reader = response.GetResponseStream())
                    {
                        await reader.CopyToAsync(content);
                    }

                    var body = Encoding.UTF8.GetString(content.ToArray());
                    var release = JsonConvert.DeserializeObject<Release>(body);
                    var githubTagVersion = release.tag_name.ToVersion();

                    if (githubTagVersion == null) return;

                    var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

                    if (githubTagVersion <= assemblyVersion) return;
                    if (Settings.Default.LastVersionPrompted.ToVersion() == githubTagVersion) return;

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
                        Process.Start(new ProcessStartInfo(release.html_url));
                    }

                    Settings.Default.LastVersionPrompted = githubTagVersion.ToString();
                    Settings.Default.Save();
                }
            }
            catch (Exception ex)
            {
                content.Dispose();
                Console.WriteLine(ex.Message);
            }
        }
    }
}
