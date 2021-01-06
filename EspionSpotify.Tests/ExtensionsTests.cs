using EspionSpotify.Enums;
using EspionSpotify.Extensions;
using EspionSpotify.Models;
using NAudio.Wave;
using System;
using System.Resources;
using Xunit;

namespace EspionSpotify.Tests
{
    public class ResourceManagerTests
    {
        private readonly ResourceManager _rm;

        public ResourceManagerTests()
        {
            _rm = new ResourceManager(Translations.Languages.getResourcesManagerLanguageType(LanguageType.en));
        }

        [Fact]
        internal void GetString_ReturnsExpectedTranslatedSpyString()
        {
            Assert.Equal("Spy", _rm.GetString(I18nKeys.TabRecord));
        }
    }

    public class StringExtensionsTest
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData("a", null)]
        [InlineData("0", 0)]
        [InlineData("1", 1)]
        internal void StringToNullableInt_ReturnsExpectedInt(string value, int? expected)
        {
            var actual = value.ToNullableInt();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData(@"\\path\home\", @"\\path\home")]
        [InlineData(@"/path/home//", @"/path/home")]
        [InlineData(@"C:\path\ ", @"C:\path")]
        internal void StringTrimEndPath_ReturnsExpectedString(string value, string expected)
        {
            var actual = value.TrimEndPath();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("", true)]
        [InlineData(Constants.SPOTIFY, true)]
        [InlineData(Constants.SPOTIFYFREE, true)]
        [InlineData(Constants.ADVERTISEMENT, true)]
        [InlineData("Spytify", false)]
        internal void StringIsNullOrInvalidSpotifyStatus_ReturnsExpectedString(string value, bool expected)
        {
            var actual = value.IsNullOrInvalidSpotifyStatus();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData("xsmall", null)]
        [InlineData("SMALL", AlbumCoverSize.small)]
        [InlineData("Small", AlbumCoverSize.small)]
        [InlineData("small", AlbumCoverSize.small)]
        [InlineData("medium", AlbumCoverSize.medium)]
        [InlineData("large", AlbumCoverSize.large)]
        [InlineData("extralarge", AlbumCoverSize.extralarge)]
        internal void StringToAlbumCoverSize_ReturnsExpectedCover(string value, AlbumCoverSize? expected)
        {
            var actual = value.ToAlbumCoverSize();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData("success",null)]
        [InlineData("failed", LastFMNodeStatus.failed)]
        [InlineData("FAILED", LastFMNodeStatus.failed)]
        [InlineData("Ok", LastFMNodeStatus.ok)]
        [InlineData("ok", LastFMNodeStatus.ok)]
        internal void StringToLastFMNodeStatus_ReturnsExpectedStatus(string value, LastFMNodeStatus? expected)
        {
            var actual = value.ToLastFMNodeStatus();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData("flac", null)]
        [InlineData("mp3", MediaFormat.Mp3)]
        [InlineData("MP3", MediaFormat.Mp3)]
        [InlineData("WAV", MediaFormat.Wav)]
        [InlineData("wav", MediaFormat.Wav)]
        internal void StringToMediaFormat_ReturnsExpectedFormat(string value, MediaFormat? expected)
        {
            var actual = value.ToMediaFormat();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData("API", null)]
        [InlineData("spotify", ExternalAPIType.Spotify)]
        [InlineData("lastFM", ExternalAPIType.LastFM)]
        internal void StringToMediaTagsAPI_ReturnsExpectedAPI(string value, ExternalAPIType? expected)
        {
            var actual = value.ToMediaTagsAPI();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData(" ", "")]
        [InlineData("v1", "1")]
        [InlineData("1", "1")]
        [InlineData("version1", "1")]
        [InlineData("v0.1-beta", "0.1")]
        [InlineData("1.123", "1.123")]
        [InlineData("1.0.10", "1.0.10")]
        [InlineData("1.2.3.4", "1.2.3.4")]
        [InlineData("v1.2.3.4", "1.2.3.4")]
        internal void StringToVersionAsString(string value, string expected)
        {
            var actual = value.ToVersionAsString();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("v.1")]
        [InlineData("v1")]
        [InlineData("1")]
        [InlineData("version1")]
        internal void StringToVersion_ReturnsNull(string value)
        {
            var actual = value.ToVersion();
            Assert.Null(actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData(" ", " ")]
        [InlineData("test", "Test")]
        [InlineData("-abc", "-abc")]
        [InlineData("123", "123")]
        [InlineData("Abc", "Abc")]
        [InlineData("aBC", "ABC")]
        [InlineData("a b c", "A b c")]
        internal void StringCapitalize_ReturnsExpected(string value, string expected)
        {
            var actual = value.Capitalize();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("v0.1-beta", 0, 1)]
        [InlineData("1.123", 1, 123)]
        [InlineData("1.1.0.0", 1, 1, 0, 0)]
        [InlineData("1.0.10", 1, 0, 10)]
        [InlineData("1.2.3.4", 1, 2, 3, 4)]
        [InlineData("v1.2.3.4", 1, 2, 3, 4)]
        internal void StringToVersion_ReturnsVersion(string value, int major, int minor, int? build = null, int? revision = null)
        {
            var actual = value.ToVersion();
            if (build == null) Assert.Equal(new Version(major, minor), actual);
            else if (revision == null) Assert.Equal(new Version(major, minor, (int)build), actual);
            else Assert.Equal(new Version(major, minor, (int)build, (int)revision), actual);
        }
    }

    public class LinqExtensionsTest
    {
        [Fact]
        internal void EmptyArrayDecimalMedian_ReturnsMedianDecimal()
        {
            var value = new double[] {};

            Assert.Throws<InvalidOperationException>(() => value.Median());
        }

        [Fact]
        internal void PeerArrayDecimalMedian_ReturnsMedianDecimal()
        {
            var value = new double[] { 2.8, 1.4, 1.1, 0.8, -0.4, 1.1, 2.4, 7.77 };
            var expected = 1.25;

            var actual = value.Median();

            Assert.Equal(expected, actual);
        }

        [Fact]
        internal void OddArrayDecimalMedian_ReturnsMedianDecimal()
        {
            var value = new double[] { 5.5, 0.9, 1.1, 0.8, -0.4, 1.11, 0.004, 2.4, 7.77 };
            var expected = 1.1;

            var actual = value.Median();

            Assert.Equal(expected, actual);
        }

        [Fact]
        internal void ArrayIntMedian_ReturnsMedianDecimal()
        {
            var value = new int[] { 5, 3, 6, 0, -1, 1, 0, 2, 7 };
            var expected = 2;

            var actual = value.Median();

            Assert.Equal(expected, actual);
        }

        [Fact]
        internal void LinqArrayDoubleMedian_ReturnsMedianDecimal()
        {
            var value = new LinqArrayDouble[] {
                new LinqArrayDouble { value = 5.5 },
                new LinqArrayDouble { value = 0.9 },
                new LinqArrayDouble { value = 1.6 }
            };
            var expected = 1.6;

            var actual = value.Median(x => x.value);

            Assert.Equal(expected, actual);
        }

        internal class LinqArrayDouble
        {
            internal double value { get; set; }
        }
    }

    public class WaveFormatExtensionsTest
    {
        [Fact]
        internal void WaveFormatRestriction_ReturnsChannelCodeOverStereo()
        {
            var waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(Recorder.MP3_MAX_SAMPLE_RATE, 6);

            Assert.Contains(WaveFormatMP3Restriction.Channel, waveFormat.GetMP3RestrictionCode());
        }

        [Fact]
        internal void WaveFormatRestriction_ReturnsSampleRateCodeOver48k()
        {
            var waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(96000, Recorder.MP3_MAX_NUMBER_CHANNELS);

            Assert.Contains(WaveFormatMP3Restriction.SampleRate, waveFormat.GetMP3RestrictionCode());
        }

        [Fact]
        internal void WaveFormatRestriction_ReturnsSampleRateAndChannelCodesOverLimit()
        {
            var waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(96000, 6);

            Assert.Equal(new[] { WaveFormatMP3Restriction.Channel, WaveFormatMP3Restriction.SampleRate }, waveFormat.GetMP3RestrictionCode());
        }

        [Fact]
        internal void WaveFormatRestriction_ReturnsNoCodeUnderIEEE()
        {
            var waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(Recorder.MP3_MAX_SAMPLE_RATE, Recorder.MP3_MAX_NUMBER_CHANNELS);

            Assert.Empty(waveFormat.GetMP3RestrictionCode());
        }
    }
}
