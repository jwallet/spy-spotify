using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EspionSpotify.Enums;

namespace EspionSpotify.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex RegexVersion = new Regex(@"(\d+\.)(\d+\.)?(\d+\.)?(\*|\d+)");
        private static readonly Regex RegexTag = new Regex(@"[^\d+\.]");

        public static AlbumCoverSize? ToAlbumCoverSize(this string value)
        {
            return value.ToEnum<AlbumCoverSize>(true);
        }

        public static LastFMNodeStatus? ToLastFMNodeStatus(this string value)
        {
            return value.ToEnum<LastFMNodeStatus>(true);
        }

        public static MediaFormat? ToMediaFormat(this string value)
        {
            return value.ToEnum<MediaFormat>(true);
        }

        public static ExternalAPIType? ToMediaTagsAPI(this string value)
        {
            return value.ToEnum<ExternalAPIType>(true);
        }

        public static IEnumerable<string> ToPerformers(this string value)
        {
            var match = new Regex(@"\((with |feat\. )(?<performers>.*)\)").Match(value);
            var performers = match.Groups["performers"].ToString().Replace(" & ", ", ");
            return Regex.Split(performers, @", ").AsEnumerable();
        }

        public static LanguageType? ToLanguageType(this string value)
        {
            return value.ToEnum<LanguageType>(true);
        }

        public static int? ToNullableInt(this string value)
        {
            if (int.TryParse(value, out var i)) return i;
            return null;
        }

        public static string TrimEndPath(this string path)
        {
            return path?.Trim()?.TrimEnd(Path.GetInvalidFileNameChars());
        }

        public static bool IsNullOrAdOrSpotifyIdleState(this string value)
        {
            return IsNullOrSpotifyIdleState(value) || IsSpotifyPlayingAnAd(value);
        }

        public static bool IsSpotifyPlayingAnAd(this string value)
        {
            return  Constants.ADVERTISEMENT.ToLowerInvariant() == value.ToLowerInvariant();
        }

        public static bool IsNullOrSpotifyIdleState(this string value)
        {
            return string.IsNullOrWhiteSpace(value) || value.IsSpotifyIdleState();
        }

        public static bool IsSpotifyIdleState(this string value)
        {
            return new[]
            {
                Constants.SPOTIFY.ToLowerInvariant(),
                Constants.SPOTIFYFREE.ToLowerInvariant(),
                Constants.SPOTIFYPREMIUM.ToLowerInvariant()
            }.Contains(value.ToLowerInvariant());
        }

        public static T? ToEnum<T>(this string value, bool ignoreCase) where T : struct
        {
            var types = typeof(T);
            if (string.IsNullOrEmpty(value) ||
                Enum.GetNames(types).All(x => x.ToLowerInvariant() != value.ToLowerInvariant())) return null;

            return (T) Enum.Parse(types, value, ignoreCase);
        }

        public static string ToVersionAsString(this string tag)
        {
            return string.IsNullOrEmpty(tag) ? string.Empty : RegexTag.Replace(tag, string.Empty);
        }

        public static Version ToVersion(this string value)
        {
            var versionString = value.ToVersionAsString();

            if (string.IsNullOrEmpty(versionString) || !RegexVersion.IsMatch(versionString)) return null;

            return new Version(versionString);
        }

        public static string Capitalize(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;
            return input.First().ToString().ToUpper() + input.Substring(1);
        }

        public static string ToMaxLength(this string input, int max = -1)
        {
            if (input.Length <= max || max == -1) return input;
            return input.Substring(0, max);
        }
    }
}