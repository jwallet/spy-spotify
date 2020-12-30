using EspionSpotify.Enums;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace EspionSpotify.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex _regexVersion = new Regex(@"(\d+\.)(\d+\.)?(\d+\.)?(\*|\d+)");
        private static readonly Regex _regexTag = new Regex(@"[^\d+\.]");

        public static AlbumCoverSize? ToAlbumCoverSize(this string value)
        {
            return value.ToEnum<AlbumCoverSize>(ignoreCase: true);
        }

        public static LastFMNodeStatus? ToLastFMNodeStatus(this string value)
        {
            return value.ToEnum<LastFMNodeStatus>(ignoreCase: true);
        }

        public static MediaFormat? ToMediaFormat(this string value)
        {
            return value.ToEnum<MediaFormat>(ignoreCase: true);
        }

        public static ExternalAPIType? ToMediaTagsAPI(this string value)
        {
            return value.ToEnum<ExternalAPIType>(ignoreCase: true);
        }

        public static int? ToNullableInt(this string value)
        {
            if (int.TryParse(value, out int i)) return i;
            return null;
        }

        public static T? ToEnum<T>(this string value, bool ignoreCase) where T : struct
        {
            var types = typeof(T);
            if (string.IsNullOrEmpty(value) || !Enum.GetNames(types).Any(x => x.ToLowerInvariant() == value.ToLowerInvariant()))
            {
                return null;
            }

            return (T)Enum.Parse(types, value, ignoreCase);
        }

        public static string ToVersionAsString(this string tag)
        {
            return string.IsNullOrEmpty(tag) ? string.Empty : _regexTag.Replace(tag, string.Empty);
        }

        public static Version ToVersion(this string value)
        {
            var versionString = value.ToVersionAsString();

            if (string.IsNullOrEmpty(versionString) || !_regexVersion.IsMatch(versionString)) return null;

            return new Version(versionString);
        }

        public static string Capitalize(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
}
