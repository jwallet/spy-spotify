using EspionSpotify.Enums;
using EspionSpotify.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EspionSpotify.Tests
{
    public class IntExtensionsTests
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData("a", null)]
        [InlineData("0", 0)]
        [InlineData("1", 1)]
        private void StringToNullableInt_ReturnsExpectedInt(string value, int? expected)
        {
            var actual = value.ToNullableInt();
            Assert.Equal(expected, actual);
        }
    }

    public class ResourceManagerTests
    {
        private readonly ResourceManager _rm;

        public ResourceManagerTests()
        {
            _rm = new ResourceManager(Translations.Languages.getResourcesManagerLanguageType(LanguageType.en));
        }

        [Fact]
        private void GetString_ReturnsExpectedTranslatedSpyString()
        {
            Assert.Equal("Spy", _rm.GetString(TranslationKeys.tabRecord));
        }
    }

    public class StringExtensionsTest
    {
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
        private void StringToAlbumCoverSize_ReturnsExpectedCover(string value, AlbumCoverSize? expected)
        {
            var actual = value.ToAlbumCoverSize();
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
        private void StringToVersionAsString(string value, string expected)
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
        private void StringToVersion_ReturnsNull(string value)
        {
            var actual = value.ToVersion();
            Assert.Null(actual);
        }

        [Theory]
        [InlineData("v0.1-beta", 0, 1)]
        [InlineData("1.123", 1, 123)]
        [InlineData("1.1.0.0", 1, 1, 0, 0)]
        [InlineData("1.0.10", 1, 0, 10)]
        [InlineData("1.2.3.4", 1, 2, 3, 4)]
        [InlineData("v1.2.3.4", 1, 2, 3, 4)]
        private void StringToVersion_ReturnsVersion(string value, int major, int minor, int? build = null, int? revision = null)
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
        private void EmptyArrayDecimalMedian_ReturnsMedianDecimal()
        {
            var value = new double[] {};

            Assert.Throws<InvalidOperationException>(() => value.Median());
        }

        [Fact]
        private void PeerArrayDecimalMedian_ReturnsMedianDecimal()
        {
            var value = new double[] { 2.8, 1.4, 1.1, 0.8, -0.4, 1.1, 2.4, 7.77 };
            var expected = 1.25;

            var actual = value.Median();

            Assert.Equal(expected, actual);
        }

        [Fact]
        private void OddArrayDecimalMedian_ReturnsMedianDecimal()
        {
            var value = new double[] { 5.5, 0.9, 1.1, 0.8, -0.4, 1.11, 0.004, 2.4, 7.77 };
            var expected = 1.1;

            var actual = value.Median();

            Assert.Equal(expected, actual);
        }

        [Fact]
        private void ArrayIntMedian_ReturnsMedianDecimal()
        {
            var value = new int[] { 5, 3, 6, 0, -1, 1, 0, 2, 7 };
            var expected = 2;

            var actual = value.Median();

            Assert.Equal(expected, actual);
        }

        [Fact]
        private void LinqArrayDoubleMedian_ReturnsMedianDecimal()
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

        private class LinqArrayDouble
        {
            internal double value { get; set; }
        }
    }
}
