using EspionSpotify.Enums;
using System;
using System.Text.RegularExpressions;

namespace EspionSpotify.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex _regexVersion = new Regex(@"(\d+\.)(\d+\.)?(\d+\.)?(\*|\d+)");
        private static readonly Regex _regexTag = new Regex(@"[^\d+\.]");

        public static AlbumCoverSize? ToAlbumCoverSize(this string value)
        {
            var types = typeof(AlbumCoverSize);

            if (string.IsNullOrEmpty(value) || !Enum.IsDefined(types, value.ToLowerInvariant()))
            {
                return null;
            }

            return (AlbumCoverSize) Enum.Parse(types, value, ignoreCase: true);
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
    }
}
