using System;
using EspionSpotify.Enums;

namespace EspionSpotify.Extensions
{
    public static class OperatingSystemExtensions
    {
        public static bool IsAtLeast(this OperatingSystem os, OSVersions version)
        {
            return os.Version.Build >= (int)version;
        }

        public static bool IsGreaterThan(this OperatingSystem os, OSVersions version)
        {
            return os.Version.Build > (int)version;
        }

        public static bool IsLessThan(this OperatingSystem os, OSVersions version)
        {
            return os.Version.Build < (int)version;
        }
    }
}