using System;

namespace EspionSpotify.AudioSessions
{
    public static class AudioUtil
    {
        public static bool IsSilence(float amplitude, sbyte threshold)
            => GetDecibelsFromAmplitude(amplitude) < threshold;

        private static double GetDecibelsFromAmplitude(float amplitude)
            => 20 * Math.Log10(Math.Abs(amplitude));
    }
}
