using System.IO;
using System.Linq;
using System.Text;

namespace EspionSpotify
{
    internal static class Normalize
    {
        public static string RemoveDiacritics(string text)
        {
            if (text == null) return string.Empty;

            var normalizedString = text.Trim().Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                if (!Path.GetInvalidFileNameChars().Contains(c))
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
