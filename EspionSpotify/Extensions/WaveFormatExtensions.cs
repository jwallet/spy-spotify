using EspionSpotify.Enums;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EspionSpotify.Extensions
{
    public static class WaveFormatExtensions
    {
        public static IEnumerable<WaveFormatMP3Restriction> GetMP3RestrictionCode(this WaveFormat waveFormat)
        {
            var restrictions = new List<WaveFormatMP3Restriction>();
            if (waveFormat.Channels > Recorder.MP3_MAX_NUMBER_CHANNELS)
            {
                restrictions.Add(WaveFormatMP3Restriction.Channel);
            }
            if (waveFormat.SampleRate > Recorder.MP3_MAX_SAMPLE_RATE)
            {
                restrictions.Add(WaveFormatMP3Restriction.SampleRate);
            }

            return restrictions;
        }
    }
}
