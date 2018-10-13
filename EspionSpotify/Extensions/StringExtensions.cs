using EspionSpotify.Enums;
using System;
using System.Text.RegularExpressions;

namespace EspionSpotify.Extensions
{
    public static class StringExtensions
    {
        private static Regex _regexVersion = new Regex(@"(\d+\.)(\d+\.)?(\d+\.)?(\*|\d+)");
        private static Regex _regexTag = new Regex(@"[^0-9.]");

        public static AlbumCoverSize? ConvertToAlbumCoverSize(this string value)
        {
            var types = typeof(AlbumCoverSize);

            if (string.IsNullOrEmpty(value) || !Enum.IsDefined(types, value))
            {
                return null;
            }

            return (AlbumCoverSize) Enum.Parse(types, value, ignoreCase: true);
        }

        public static Version ToVersion(this string value)
        {
            var versionString = value.ToStringVersion();

            if (string.IsNullOrEmpty(versionString) || !_regexVersion.IsMatch(versionString)) return null;

            return new Version(value);
        }

        public static string ToStringVersion(this string tag)
        {
            return string.IsNullOrEmpty(tag) ? string.Empty : _regexTag.Replace(tag, string.Empty);
        }

    }
}
