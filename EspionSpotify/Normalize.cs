using System.Linq;
using System.Text;

namespace EspionSpotify
{
    internal static class Normalize
    {
        internal static readonly char[] reservedSystemChars = { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };

        public static string RemoveDiacritics(string text)
        {
            if (text == null) return string.Empty;

            var normalizedString = text.Trim().Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                if (!reservedSystemChars.Contains(c))
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
