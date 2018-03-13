using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace EspionSpotify
{
    internal static class GitHubApi
    {
        public static void NewestVersion()
        {
            Uri uri;
            
            if (!Uri.TryCreate("http://github.com/jwallet/spy-spotify/releases/latest", UriKind.Absolute, out uri)) return;

            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            var request = WebRequest.Create(uri);
            request.Method = WebRequestMethods.Http.Head;

            try
            {
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    var split = response.ResponseUri.AbsolutePath.Split('/');
                    var regex = new Regex(@"(\d+\.)(\d+\.)?(\d+\.)?(\*|\d+)");
                    var tag = split[split.Length - 1];

                    if (!regex.IsMatch(tag)) return;
                    var version = new Version(tag);

                    if (version <= Assembly.GetExecutingAssembly().GetName().Version) return;
                    if (MetroFramework.MetroMessageBox.Show(
                        FrmEspionSpotify.Instance,
                        string.Format(FrmEspionSpotify.Rm.GetString($"msgNewVersionContent") ?? "DOWLOAD {0}", version),
                        FrmEspionSpotify.Rm.GetString($"msgNewVersionTitle"),
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo(uri.AbsoluteUri));
                    }
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
