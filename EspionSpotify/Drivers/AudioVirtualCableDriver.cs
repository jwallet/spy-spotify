using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EspionSpotify.Drivers
{
    public class AudioVirtualCableDriver
    {
        const string DriverName = "VB-Audio Virtual Cable";

        public static string Path { get => $@"{Environment.CurrentDirectory}\Drivers\VBCABLE_Setup{(Environment.Is64BitOperatingSystem ? "_x64" : "")}.exe"; }

        public static bool IsFound { get => System.IO.File.Exists(Path); }

        public static bool ExistsInAudioEndPointDevices(MMDeviceCollection audioEndPointDevices)
        {
            return audioEndPointDevices.Any(x => x.DeviceFriendlyName.Equals(DriverName));
        }
        
        public static async Task<bool> SetupDriver()
        {
            try
            {
                var psi = new ProcessStartInfo()
                {
                    CreateNoWindow = false,
                    UseShellExecute = true,
                    Verb = "runas",
                    FileName = Path,
                };
                var process = new Process
                {
                    StartInfo = psi,
                };
                await Task.Run(() =>
                {
                    process.Start();
                    process.WaitForExit();
                });
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
