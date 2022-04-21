using System;
using EspionSpotify.Enums;
using EspionSpotify.Extensions;

namespace EspionSpotify.Router.Helpers
{
    public static class AudioPolicyConfigFactory
    {
        public static IAudioPolicyConfigFactory Create()
        {
            if (Environment.OSVersion.IsAtLeast(OSVersions.Version21H2))
            {
                return new AudioPolicyConfigFactoryImplFor21H2();
            }
            
            return new AudioPolicyConfigFactoryImplForDownlevel();
        }
    }
}