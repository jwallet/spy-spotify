using System;

namespace EspionSpotify.Updater.Utilities
{
    internal static class LongExtensions
    {
        private const int BYTES_IN_MEGABYTE = 1_000_000;

        internal static string ToMo(this long bytes)
        {
            return Math.Round(bytes / (double) BYTES_IN_MEGABYTE, 2).ToString("0.00");
        }
    }
}