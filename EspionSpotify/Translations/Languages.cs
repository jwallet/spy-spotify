using EspionSpotify.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EspionSpotify.Translations
{
    public static class Languages
    {
        public static readonly Dictionary<LanguageType, Type> availableResourcesManager = new Dictionary<LanguageType, Type>
        {
            { LanguageType.en, typeof(en) },
            { LanguageType.fr, typeof(fr) },
            { LanguageType.nl, typeof(nl) }
        };

        public static Type getResourcesManagerLanguageType(LanguageType languageType)
        {
            return availableResourcesManager.Where(x => x.Key.Equals(languageType)).Select(x => x.Value).FirstOrDefault();
        }

        internal static readonly Dictionary<LanguageType, string> dropdownListValues = new Dictionary<LanguageType, string>
        {
            { LanguageType.en, "English" },
            { LanguageType.fr, "Français" },
            { LanguageType.nl, "Nederlands" }
        };
    }
}
