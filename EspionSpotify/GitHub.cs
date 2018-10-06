using EspionSpotify.Models;
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
        private static Regex _regex = new Regex(@"(\d+\.)(\d+\.)?(\d+\.)?(\*|\d+)");

        public static async void NewestVersion()
        {
            if (!Uri.TryCreate("https://api.github.com/repos/jwallet/spy-spotify/releases/latest", UriKind.Absolute, out var uri)) return;

            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
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
                    var release = JsonConvert.DeserializeObject<GitHubRelease>(body);

                    if (!_regex.IsMatch(release.tag_name)) return;

                    var githubTagVersion = new Version(release.tag_name);
                    var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

                    if (githubTagVersion <= assemblyVersion) return;
                    if (LastVersionPrompted() == assemblyVersion) return;

                    var dialogTitle = $"{string.Format(FrmEspionSpotify.Rm.GetString($"msgNewVersionTitle"), githubTagVersion)}";
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

        public static Version LastVersionPrompted()
        {
            var lastVersionPromptedSaved = Settings.Default.LastVersionPrompted;
            if (!_regex.IsMatch(lastVersionPromptedSaved)) return null;

            return new Version(lastVersionPromptedSaved);
        }
    }
}
