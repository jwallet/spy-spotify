using System;
using System.IO;
using System.Linq;
using EspionSpotify.Properties;

namespace EspionSpotify
{
    internal class ManageHosts
    {
        public static readonly string HostsSystemPath = @"\System32\drivers\etc\hosts";
        private static readonly string BasePath = Environment.GetEnvironmentVariable("WINDIR");
        private static readonly string[] AdHosts = Settings.Default.AdHosts.Split(';');
        private static readonly string HostsTitle = $"# Spotify Disabled Ad Hosts";

        public static bool EnableAds(string hosts) => UpdateHosts(hosts, true);
        public static bool DisableAds(string hosts) => UpdateHosts(hosts);
        public static bool AreAdsDisabled(string hosts) => IsDisabledAdsTitleInHost(hosts);

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

                sw.Write($"\r\n{HostsTitle}");
                foreach (var h in AdHosts)
                {
                    sw.Write($"\r\n0.0.0.0\t{h}");
                }
            }
        }

        private static bool UpdateHosts(string file, bool reset = false)
        {
            if (!ValidAccess.ToFile(BasePath, file)) return false;

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

        private static bool IsDisabledAdsTitleInHost(string file)
        {
            var allEntries = ReadToFile(file);
            var fileContent = allEntries.Replace("\r", string.Empty).Split('\n');
            return fileContent.Any(line => line.Contains(HostsTitle));
        }

        private static string FilterEntries(string content)
        {
            var newContent = string.Empty;
            var fileContent = content.Replace("\r", string.Empty).Split('\n');
            newContent = fileContent.Where(line => !AdHosts.Any(line.Contains) && !line.Contains(HostsTitle)).Aggregate(newContent, (current, line) => current + line.Replace("\n", string.Empty) + "\r\n");

            return newContent.TrimEnd();
        }
    }
}