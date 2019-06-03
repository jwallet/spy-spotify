using EspionSpotify.Enums;
using System;
using System.Collections.Generic;

namespace EspionSpotify.Translations
{
    internal static class Languages
    {
        internal static readonly Dictionary<LanguageType, Type> availableResourcesManager = new Dictionary<LanguageType, Type>
        {
            { LanguageType.en, typeof(en) },
            { LanguageType.fr, typeof(fr) }
        };

        internal static readonly Dictionary<LanguageType, string> dropdownListValues = new Dictionary<LanguageType, string>
        {
            { LanguageType.en, "English" },
            { LanguageType.fr, "Français" }
        };
    }
}
