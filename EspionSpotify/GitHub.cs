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
        private static Regex _regexVersion = new Regex(@"(\d+\.)(\d+\.)?(\d+\.)?(\*|\d+)");
        private static Regex _regexTag = new Regex(@"[^0-9.]");
        private const string _repoReleaseLink = "https://api.github.com/repos/jwallet/spy-spotify/releases/latest";

        public static async void NewestVersion()
        {
            if (!Uri.TryCreate(_repoReleaseLink, UriKind.Absolute, out var uri)) return;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = WebRequestMethods.Http.Get;
            request.UserAgent = "Spytify";

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
                    var versionTag = ParseTagToVersionString(release.tag_name);

                    if (!_regexVersion.IsMatch(versionTag)) return;

                    var githubTagVersion = new Version(versionTag);
                    var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

                    if (githubTagVersion <= assemblyVersion) return;
                    if (LastVersionPrompted() == assemblyVersion) return;

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
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo(release.html_url));
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        Settings.Default.LastVersionPrompted = assemblyVersion.ToString();
                        Settings.Default.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                content.Dispose();
                Console.WriteLine(ex.Message);
            }
        }

        private static Version LastVersionPrompted()
        {
            var lastVersionPromptedSaved = Settings.Default.LastVersionPrompted;
            if (!_regexVersion.IsMatch(lastVersionPromptedSaved)) return null;

            return new Version(lastVersionPromptedSaved);
        }

        private static string ParseTagToVersionString(string tag)
        {
            return string.IsNullOrEmpty(tag) ? string.Empty : _regexTag.Replace(tag, string.Empty);
        }
    }
}
