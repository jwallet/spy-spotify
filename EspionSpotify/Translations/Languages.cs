using EspionSpotify.Enums;
using EspionSpotify.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EspionSpotify.Translations
{
    public static class Languages
    {
        public static readonly Dictionary<LanguageType, Type> availableResourcesManager = new Dictionary<LanguageType, Type>
        {
            { LanguageType.cs, typeof(cs) },
            { LanguageType.de, typeof(de) },
            { LanguageType.en, typeof(en) },
            { LanguageType.es, typeof(es) },
            { LanguageType.fr, typeof(fr) },
            { LanguageType.it, typeof(it) },
            { LanguageType.ja, typeof(ja) },
            { LanguageType.nl, typeof(nl) },
            { LanguageType.pl, typeof(pl) },
            { LanguageType.pt, typeof(pt) },
            { LanguageType.ru, typeof(ru) },
            { LanguageType.tr, typeof(tr) },
        };

        public static Type GetResourcesManagerLanguageType(LanguageType? type)
        {
            return availableResourcesManager.Where(x => x.Key.Equals(type ?? LanguageType.en)).Select(x => x.Value).Single();
        }

        public static KeyValuePair<LanguageType, string> GetDropdownListItemFromLanguageType(LanguageType? type)
        {
            return dropdownListValues.Single(x => x.Key == (type ?? LanguageType.en));
        }

        internal static readonly Dictionary<LanguageType, string> dropdownListValues = new Dictionary<LanguageType, string>
        {
            { LanguageType.en, "English" },
            //{ LanguageType.cs, "Čeština" }, // Czech
            //{ LanguageType.de, "Deutsch" }, // German
            //{ LanguageType.es, "Española" }, // Spanish
            { LanguageType.fr, "Français" }, // French
            //{ LanguageType.it, "Italiano" }, // Italian
            //{ LanguageType.ja, "Nihonjin" }, // Japanese
            //{ LanguageType.nl, "Nederlands" }, // Dutch
            //{ LanguageType.pl, "Polskie" }, // Polish
            //{ LanguageType.pt, "Português" }, // Portuguese
            //{ LanguageType.ru, "Roshia" }, // Russian
            //{ LanguageType.tr, "Türk" }, // Turkish
        };
    }
}
