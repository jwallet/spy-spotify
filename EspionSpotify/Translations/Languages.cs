using System;
using System.Collections.Generic;
using System.Linq;
using EspionSpotify.Enums;

namespace EspionSpotify.Translations
{
    public static class Languages
    {
        private static readonly Dictionary<LanguageType, Type> AvailableResourcesManager =
            new Dictionary<LanguageType, Type>
            {
                {LanguageType.en, typeof(en)},
                {LanguageType.fr, typeof(fr)}
            };

        internal static readonly Dictionary<LanguageType, string> DropdownListValues =
            new Dictionary<LanguageType, string>
            {
                {LanguageType.en, "English"},
                {LanguageType.fr, "Français"} // French
            };

        public static Type GetResourcesManagerLanguageType(LanguageType? type)
        {
            return AvailableResourcesManager.Where(x => x.Key.Equals(type ?? LanguageType.en)).Select(x => x.Value)
                .Single();
        }

        public static KeyValuePair<LanguageType, string> GetDropdownListItemFromLanguageType(LanguageType? type)
        {
            return DropdownListValues.Single(x => x.Key == (type ?? LanguageType.en));
        }
    }
}