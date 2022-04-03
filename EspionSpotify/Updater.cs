using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using EspionSpotify.Properties;

namespace EspionSpotify
{
    internal static class Updater
    {
        private const string UPDATER_DIRECTORY = "Updater";
        private const string UPDATER_TMP_DIRECTORY = "tmp_updater";
        private static readonly string ProjectDirectory = AppDomain.CurrentDomain.BaseDirectory;

        private static readonly string UpdaterExtractedTempDirectoryPath = $"{ProjectDirectory}{UPDATER_TMP_DIRECTORY}";

        private static readonly string UpdaterExtractedContentDirectoryPath =
            $@"{UpdaterExtractedTempDirectoryPath}\{UPDATER_DIRECTORY}";

        private static readonly string UpdaterDirectoryPath = $"{ProjectDirectory}{UPDATER_DIRECTORY}";

        internal static void UpgradeSettings()
        {
            if (!Directory.Exists(UpdaterExtractedContentDirectoryPath)) return;
            Settings.Default.Upgrade();
            Settings.Default.Save();
            UpdateDirectoryContent();
            DeleteOlderUserSettings();
        }

        private static void UpdateDirectoryContent()
        {
            try
            {
                Directory.Delete(UpdaterDirectoryPath, true);
                Directory.Move(UpdaterExtractedContentDirectoryPath, UpdaterDirectoryPath);
                Directory.Delete(UpdaterExtractedTempDirectoryPath);
            }
            catch
            {
                // ignored
            }
        }

        private static void DeleteOlderUserSettings()
        {
            var path = Path.GetFullPath(Path.Combine(
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath,
                @"..\..\"));
            var settingPaths = Directory.GetDirectories(path);
            foreach (var settingPath in settingPaths)
            {
                var info = new DirectoryInfo(settingPath);
                if (info.Name != Application.ProductVersion) Directory.Delete(settingPath, true);
            }
        }
    }
}