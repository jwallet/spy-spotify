using System;
using EspionSpotify.Native;
using EspionSpotify.Router.Helpers;
using NAudio.CoreAudioApi;

namespace EspionSpotify.Router
{
    /// <summary>
    /// Provides a service for controlling and interacting with the audio device of an application.
    /// </summary>
    public class AudioRouter: IAudioRouter
    {
        /// <summary>
        /// The device interface string represents audio playback.
        /// </summary>
        private const string DEVINTERFACE_AUDIO_RENDER = "#{e6327cad-dcec-4949-ae8a-991e976a79d2}";

        /// <summary>
        /// The device interface string represents audio recording.
        /// </summary>
        private const string DEVINTERFACE_AUDIO_CAPTURE = "#{2eef81be-33fa-4800-9670-1cd474972c3f}";

        /// <summary>
        /// The MMDevice API token.
        /// </summary>
        private const string MMDEVAPI_TOKEN = @"\\?\SWD#MMDEVAPI#";

        private IAudioPolicyConfigFactory _sharedPolicyConfig;
        private DataFlow _flow;
        
        public AudioRouter(DataFlow flow)
        {
            _flow = flow;
        }

        private void EnsurePolicyConfig()
        {
            if (_sharedPolicyConfig == null)
            {
                _sharedPolicyConfig = AudioPolicyConfigFactory.Create();
            }
        }

        private string GenerateDeviceId(string deviceId)
        {
            return
                $"{MMDEVAPI_TOKEN}{deviceId}{(_flow == DataFlow.Render ? DEVINTERFACE_AUDIO_RENDER : DEVINTERFACE_AUDIO_CAPTURE)}";
        }

        private string UnpackDeviceId(string deviceId)
        {
            if (deviceId.StartsWith(MMDEVAPI_TOKEN)) deviceId = deviceId.Remove(0, MMDEVAPI_TOKEN.Length);
            if (deviceId.EndsWith(DEVINTERFACE_AUDIO_RENDER))
                deviceId = deviceId.Remove(deviceId.Length - DEVINTERFACE_AUDIO_RENDER.Length);
            if (deviceId.EndsWith(DEVINTERFACE_AUDIO_CAPTURE))
                deviceId = deviceId.Remove(deviceId.Length - DEVINTERFACE_AUDIO_CAPTURE.Length);
            return deviceId;
        }

        public void SetDefaultEndPoint(string deviceId, int processId)
        {
            Console.WriteLine($"AudioPolicyConfigService SetDefaultEndPoint {deviceId} {processId}");
            try
            {
                EnsurePolicyConfig();

                var hstring = IntPtr.Zero;

                if (!string.IsNullOrWhiteSpace(deviceId))
                {
                    var str = GenerateDeviceId(deviceId);
                    Combase.WindowsCreateString(str, (uint) str.Length, out hstring);
                }

                _sharedPolicyConfig.SetPersistedDefaultAudioEndpoint((uint) processId, _flow, Role.Multimedia,
                    hstring);
                _sharedPolicyConfig.SetPersistedDefaultAudioEndpoint((uint) processId, _flow, Role.Console, hstring);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
            }
        }

        public string GetDefaultEndPoint(int processId)
        {
            try
            {
                EnsurePolicyConfig();

                _sharedPolicyConfig.GetPersistedDefaultAudioEndpoint((uint) processId, _flow,
                    role: Role.Multimedia | Role.Console, out string deviceId);
                return UnpackDeviceId(deviceId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
            }

            return null;
        }

        public void ResetDefaultEndpoints()
        {
            try
            {
                EnsurePolicyConfig();

                _sharedPolicyConfig.ClearAllPersistedApplicationDefaultEndpoints();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
            }
        }
    }
}