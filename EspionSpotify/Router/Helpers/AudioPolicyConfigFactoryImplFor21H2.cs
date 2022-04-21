using System;
using EspionSpotify.Native;
using EspionSpotify.Native.Models;
using NAudio.CoreAudioApi;

namespace EspionSpotify.Router.Helpers
{
    public class AudioPolicyConfigFactoryImplFor21H2: IAudioPolicyConfigFactory
    {
        private readonly IAudioPolicyConfigFactory21H2 _factory;

        internal AudioPolicyConfigFactoryImplFor21H2()
        {
            var iid = typeof(IAudioPolicyConfigFactory21H2).GUID;
            Combase.RoGetActivationFactory("Windows.Media.Internal.AudioPolicyConfig", ref iid, out object factory);
            _factory = (IAudioPolicyConfigFactory21H2)factory;
        }

        public HRESULT ClearAllPersistedApplicationDefaultEndpoints()
        {
            return _factory.ClearAllPersistedApplicationDefaultEndpoints();
        }

        public HRESULT GetPersistedDefaultAudioEndpoint(uint processId, DataFlow flow, Role role, out string deviceId)
        {
            return _factory.GetPersistedDefaultAudioEndpoint(processId, flow, role, out deviceId);
        }

        public HRESULT SetPersistedDefaultAudioEndpoint(uint processId, DataFlow flow, Role role, IntPtr deviceId)
        {
            return _factory.SetPersistedDefaultAudioEndpoint(processId, flow, role, deviceId);
        }
    }
}