using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using SpotifyAPI.Local;

namespace EspionSpotify
{
    internal static class Spotify
    {
        public static SpotifyLocalAPI Instance = new SpotifyLocalAPI();
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

        public static void Connect()
        {
            if (!SpotifyLocalAPI.IsSpotifyInstalled()) return;

            for (var tries = 5; tries > 0; tries--)
            {
                if (RunSpotify()) break;
                
                System.Threading.Thread.Sleep(RunSpotifyInterval);
            }

            try
            {
                Instance.Connect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private static bool RunSpotify()
        {
            if (!SpotifyLocalAPI.IsSpotifyRunning())
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

            return SpotifyLocalAPI.IsSpotifyRunning();
        }

        public static bool IsConnected()
        {
            try
            {
                return Instance.GetStatus() != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

    }
}
