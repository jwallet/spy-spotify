using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace EspionSpotify.Updater.Utilities
{
    internal class AssemblyInfo
    {
        internal static bool IsNewerVersionThanCurrent(string name)
        {
            var m = Regex.Match(name, @"((?:\d+\.)*\d+?)");

            if (m.Groups[1].Success)
            {
                var availableVersion = new Version(m.Groups[1].Value);
                Version currentVersion;
                try
                {
                    currentVersion = new Version(FileVersionInfo.GetVersionInfo(Updater.AppFullPath).FileVersion);
                }
                catch
                {
                    return true;
                }

                if (availableVersion > currentVersion)
                {
                    return true;
                }

                return false;
            }

            return false;
        }
    }
}
