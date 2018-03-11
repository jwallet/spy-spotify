using System;
using System.IO;
using System.Linq;
using System.Text;
using EspionSpotify.Properties;

namespace EspionSpotify
{
    internal class ManageHosts
    {
        public static readonly string HostsSystemPath = @"\System32\drivers\etc\hosts";
        private static readonly string BasePath = Environment.GetEnvironmentVariable("WINDIR");
        private static readonly string[] AdHosts = Settings.Default.AdHosts.Split(';');

        public static bool EnableAds(string hosts) => UpdateHosts(hosts, true);
        public static bool DisableAds(string hosts) => UpdateHosts(hosts);

        private static string ReadToFile(string file)
        {
            using (var sr = new StreamReader($"{BasePath}{file}"))
            {
                return sr.ReadToEnd();
            }
        }

        private static void WriteToFile(string file, string filtredEntries, bool reset)
        {
            using (var sw = new StreamWriter($"{BasePath}{file}"))
            {
                sw.Write(filtredEntries);
                if (reset) return;

                foreach (var h in AdHosts)
                {
                    sw.Write($"\r\n0.0.0.0\t{h}");
                }
            }
        }

        private static bool UpdateHosts(string file, bool reset = false)
        {
            if (!ValidAccess.ToFile(BasePath, file, reset)) return false;

            var isReadOnly = ValidAccess.IsReadOnly(BasePath, file);
            if (isReadOnly)
            {
                ValidAccess.RemoveReadOnly(BasePath, file);
            }

            var filePath = $"{BasePath}{file}";
            File.Copy($"{filePath}", $"{filePath}.bak", true);

            var allEntries = ReadToFile(file);
            var filtredEntries = FilterEntries(allEntries);
            WriteToFile(file, filtredEntries, reset);

            if (isReadOnly)
            {
                ValidAccess.AddReadOnly(BasePath, file);
            }

            return true;
        }

        private static string FilterEntries(string content)
        {
            var newContent = string.Empty;
            var file = content.Replace("\r", string.Empty).Split('\n');
            newContent = file
                .Where(s => !AdHosts.Any(s.Contains))
                .Aggregate(newContent, (current, s) => current + s.Replace("\n", string.Empty) + "\r\n");

            return newContent.TrimEnd();
        }
    }
}