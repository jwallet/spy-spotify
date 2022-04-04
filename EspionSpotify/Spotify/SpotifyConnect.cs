using System;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace EspionSpotify.Spotify
{
    public static class SpotifyConnect
    {
        private static readonly TimeSpan RunSpotifyInterval = TimeSpan.FromSeconds(3);

        private static readonly string[] SpotifyPossiblePaths =
        {
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData),
                @"spotify\spotify.exe"),
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                @"Microsoft\WindowsApps\Spotify.exe")
        };

        public static async Task Run(IFileSystem fileSystem)
        {
            if (!IsSpotifyInstalled(fileSystem)) return;

            for (var tries = 5; tries > 0; tries--)
            {
                if (RunSpotify(fileSystem)) break;

                await Task.Delay(RunSpotifyInterval);
            }
        }

        private static bool RunSpotify(IFileSystem fileSystem)
        {
            if (!IsSpotifyRunning())
                try
                {
                    foreach (var path in SpotifyPossiblePaths)
                    {
                        if (!fileSystem.File.Exists(path)) continue;
                        Process.Start(path);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }

            return IsSpotifyRunning();
        }

        public static bool IsSpotifyInstalled(IFileSystem fileSystem)
        {
            return SpotifyPossiblePaths.Any(path => fileSystem.File.Exists(path));
        }

        public static bool IsSpotifyRunning()
        {
            try
            {
                return Process.GetProcessesByName(Constants.SPOTIFY).Length >= 1;
            }
            catch
            {
                return false;
            }
        }
    }
}