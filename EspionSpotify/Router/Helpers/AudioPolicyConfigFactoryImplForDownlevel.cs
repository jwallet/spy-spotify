using System;
using EspionSpotify.Native;
using EspionSpotify.Native.Models;
using NAudio.CoreAudioApi;

namespace EspionSpotify.Router.Helpers
{
    public class AudioPolicyConfigFactoryImplForDownlevel: IAudioPolicyConfigFactory
    {
        private readonly IAudioPolicyConfigFactoryDownlevel _factory;

        internal AudioPolicyConfigFactoryImplForDownlevel()
        {
            var iid = typeof(IAudioPolicyConfigFactoryDownlevel).GUID;
            Combase.RoGetActivationFactory("Windows.Media.Internal.AudioPolicyConfig", ref iid, out object factory);
            _factory = (IAudioPolicyConfigFactoryDownlevel)factory;
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