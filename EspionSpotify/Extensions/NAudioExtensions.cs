using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EspionSpotify.Extensions
{
    public static class NAudioExtensions
    {
        public static string GetFriendlyName(this MMDevice device)
        {
            string result;
            try { result = device.FriendlyName; }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            return result;
        }

        public static MMDevice GetDefaultAudioEndpointSafeException(this MMDeviceEnumerator audioMMDevices, DataFlow dataflow, Role deviceRole, bool safe = true)
        {
            MMDevice device;
            try
            {
                device = audioMMDevices.GetDefaultAudioEndpoint(dataflow, deviceRole);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (!safe)
                {
                    var message = "Failed to use your default audio device as a capture device. This can occur if your device has been unplugged, or the audio hardware have been reconfigured, disabled, removed, or otherwise made unavailable for use.";
                    var exception = new Exception(message, ex);
                    Program.ReportException(exception);
                }
                return null;
            }
            return device;
        }

        public static MMDevice GetDeviceSafeException(this MMDeviceEnumerator audioMMDevices, string deviceName, bool safe = true)
        {
            MMDevice device;
            try
            {
                device = audioMMDevices.GetDevice(deviceName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (!safe)
                {
                    var message = "Failed to use the specified audio device as a capture device.";
                    var exception = new Exception(message, ex);
                    Program.ReportException(exception);
                }
                return null;
            }
            return device;
        }
    }
}
