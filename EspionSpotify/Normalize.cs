using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq;

namespace EspionSpotify
{
    internal static class Normalize
    {
        private static char[] _reservedSystemChars = { '\\','/',':','*','?', '"', '<', '>', '|'};

        public static string RemoveDiacritics(string text)
        {
            if (text == null) return string.Empty;

            var normalizedString = text.Trim().Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                if (!_reservedSystemChars.Contains(c))
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
