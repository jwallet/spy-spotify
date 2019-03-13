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
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace EspionSpotify
{
    internal static class GitHub
    {
        private const string REPO_RELEASE_LINK = "https://api.github.com/repos/jwallet/spy-spotify/releases/latest";
        private const string SPYTIFY = "Spytify";

        public static async void GetVersion()
        {
            if (!Uri.TryCreate(REPO_RELEASE_LINK, UriKind.Absolute, out var uri)) return;

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

                    var dialogTitle = string.Format(FrmEspionSpotify.Rm.GetString($"msgNewVersionTitle"), githubTagVersion);
                    var dialogMessage = FrmEspionSpotify.Rm.GetString($"msgNewVersionContent");

                    if (!string.IsNullOrEmpty(release.body))
                    {
                        var releaseBodySplitted = release.body.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                        dialogMessage = $"{releaseBodySplitted.Take(5).Aggregate((current, next) => $"{current}\n{next}")}\r\n{dialogMessage}";
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
