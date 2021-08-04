using EspionSpotify.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EspionSpotify
{
    internal class Updater
    {
        internal const string UPDATER_DIRECTORY = "Updater";
        internal const string UPDATER_TMP_DIRECTORY = "tmp_updater";
        internal static string ProjectDirectory = AppDomain.CurrentDomain.BaseDirectory;

        internal static string UpdaterExtractedTempDirectoryPath = $"{ProjectDirectory}{UPDATER_TMP_DIRECTORY}";
        internal static string UpdaterExtractedContentDirectoryPath = $@"{UpdaterExtractedTempDirectoryPath}\{UPDATER_DIRECTORY}";
        internal static string UpdaterDirectoryPath = $"{ProjectDirectory}{UPDATER_DIRECTORY}";

        internal static void UpgradeSettings()
        {
            if (!Directory.Exists(UpdaterExtractedContentDirectoryPath)) return;
            Settings.Default.Upgrade();
            Settings.Default.Save();
            UpdateDirectoryContent();
            // DeleteOlderUserSettings();
        }
        private static void UpdateDirectoryContent()
        {
            try
            {
                Directory.Delete(UpdaterDirectoryPath, recursive: true);
                Directory.Move(UpdaterExtractedContentDirectoryPath, UpdaterDirectoryPath);
                Directory.Delete(UpdaterExtractedTempDirectoryPath);
            }
            catch { }
        }

        private static void DeleteOlderUserSettings()
        {
            var path = Path.GetFullPath(Path.Combine(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath, @"..\..\"));
            var settingPaths = Directory.GetDirectories(path);
            foreach (var settingPath in settingPaths)
            {
                DirectoryInfo info = new DirectoryInfo(settingPath);
                if (info.Name != Application.ProductVersion)
                {
                    Directory.Delete(settingPath, true);
                }
            }
        }
    }
}
