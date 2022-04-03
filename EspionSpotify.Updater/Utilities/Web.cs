using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EspionSpotify.Updater.Utilities
{
    internal class Web
    {
        internal const string USER_AGENT =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.84 Safari/537.36";

        internal static async Task<string> DownloadFileAsync(string url, string tag)
        {
            var uri = new Uri(url);
            Console.WriteLine("Downloading update {0}...", tag);

            var fileName = $"{Updater.ProjectDirectory}update-{tag}.zip";

            using (var wc = new WebClient())
            {
                wc.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                wc.DownloadFileCompleted += WebClient_DonwloadFileCompleted;

                await wc.DownloadFileTaskAsync(uri, fileName);
            }

            return fileName;
        }

        internal static void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            lock (e.UserState)
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.WriteLine("{0} % completed ({1} of {2} Mo.)",
                    e.ProgressPercentage,
                    e.BytesReceived.ToMo(),
                    e.TotalBytesToReceive.ToMo());
            }
        }

        internal static void WebClient_DonwloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled) Console.WriteLine("File download cancelled.");

            if (e.Error != null) Console.WriteLine(e.Error.ToString());
        }

        internal static async Task<string> SendWebRequestAndGetContentAsync(string address)
        {
            var result = string.Empty;
            var content = new MemoryStream();
            var myHttpWebRequest = SetHttpWebRequest(address);
            myHttpWebRequest.Method = WebRequestMethods.Http.Get;

            try
            {
                using (var myHttpWebResponse = (HttpWebResponse) await myHttpWebRequest.GetResponseAsync())
                {
                    if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
                        using (var reader = myHttpWebResponse.GetResponseStream())
                        {
                            await reader.CopyToAsync(content);
                            result = Encoding.UTF8.GetString(content.ToArray());
                        }
                }
            }
            catch
            {
            }
            finally
            {
                content.Dispose();
            }

            return result;
        }

        private static HttpWebRequest SetHttpWebRequest(string address)
        {
            Uri uriGitHub;

            if (!Uri.TryCreate(address, UriKind.Absolute, out uriGitHub)) return null;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var request = (HttpWebRequest) WebRequest.Create(uriGitHub);
            request.Method = WebRequestMethods.Http.Get;
            request.UserAgent = "Spytify";

            return request;
        }
    }
}