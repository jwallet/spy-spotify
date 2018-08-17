using System;
using System.Diagnostics;
using System.IO;

namespace EspionSpotify
{
    public class SpotifyConnect
    {
        private const string _spotify = "spotify";
        private static readonly TimeSpan RunSpotifyInterval = TimeSpan.FromSeconds(3);

        private static readonly string[] SpotifyPossiblePaths =
        {
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData),
                "spotify\\spotify.exe"),
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.
                        LocalApplicationData),
                "Microsoft\\WindowsApps\\Spotify.exe")
        };

        public static void Run()
        {
            if (!IsSpotifyInstalled())
            {
                return;
            }

            for (var tries = 5; tries > 0; tries--)
            {
                if (RunSpotify()) break;

                System.Threading.Thread.Sleep(RunSpotifyInterval);
            }
        }

        private static bool RunSpotify()
        {
            if (!IsSpotifyRunning())
            {
                try
                {
                    foreach (var path in SpotifyPossiblePaths)
                    {
                        if (!File.Exists(path)) continue;
                        Process.Start(path);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }

            return IsSpotifyRunning();
        }

        public static bool IsSpotifyInstalled()
        {
            foreach (var path in SpotifyPossiblePaths)
            {
                if (!File.Exists(path)) continue;
                return true;
            }
            return false;
        }

        public static bool IsSpotifyRunning()
        {
            return Process.GetProcessesByName(_spotify).Length >= 1;
        }
    }
}
