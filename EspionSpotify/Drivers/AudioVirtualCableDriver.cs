using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace EspionSpotify.Drivers
{
    public static class AudioVirtualCableDriver
    {
        private const string DRIVER_NAME = "VB-Audio Virtual Cable";

        private static string Path =>
            $@"{Environment.CurrentDirectory}\Drivers\VBCABLE_Setup{(Environment.Is64BitOperatingSystem ? "_x64" : "")}.exe";

        public static bool IsFound => File.Exists(Path);

        public static bool ExistsInAudioEndPointDevices(IDictionary<string, string> audioEndPointDeviceNames)
        {
            return audioEndPointDeviceNames.Any(x => x.Value.Contains(DRIVER_NAME));
        }

        public static bool SetupDriver()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    CreateNoWindow = false,
                    UseShellExecute = true,
                    Verb = "runas",
                    FileName = Path
                };
                var process = new Process
                {
                    StartInfo = psi
                };
                process.Start();
                process.WaitForExit();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}