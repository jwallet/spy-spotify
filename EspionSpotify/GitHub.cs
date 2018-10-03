using EspionSpotify.Properties;
using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace EspionSpotify
{
    internal static class GitHub
    {
        private static Regex _regex = new Regex(@"(\d+\.)(\d+\.)?(\d+\.)?(\*|\d+)");

        public static void NewestVersion()
        {
            if (!Uri.TryCreate("http://github.com/jwallet/spy-spotify/releases/latest", UriKind.Absolute, out var uri)) return;

            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            var request = WebRequest.Create(uri);
            request.Method = WebRequestMethods.Http.Head;

            try
            {
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    var split = response.ResponseUri.AbsolutePath.Split('/');
                    var tag = split[split.Length - 1];
                    if (!_regex.IsMatch(tag)) return;

                    var githubTagVersion = new Version(tag);
                    var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

                    if (githubTagVersion <= assemblyVersion) return;
                    if (LastVersionPrompted() == assemblyVersion) return;

                    var dialogResult = MetroFramework.MetroMessageBox.Show(
                        FrmEspionSpotify.Instance,
                        string.Format(FrmEspionSpotify.Rm.GetString($"msgNewVersionContent") ?? "DOWNLOAD {0}", githubTagVersion),
                        FrmEspionSpotify.Rm.GetString($"msgNewVersionTitle"),
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo(uri.AbsoluteUri));
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
