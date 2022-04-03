using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Resources;
using EspionSpotify.Extensions;
using Xunit;

namespace EspionSpotify.Tests
{
    public class TranslationTests
    {
        private readonly Type _en;
        private readonly Type _fr;
        private static ResourceManager RM;
        private readonly int _keysCount;

        public TranslationTests()
        {
            _en = Translations.Languages.GetResourcesManagerLanguageType(Enums.LanguageType.en);
            _fr = Translations.Languages.GetResourcesManagerLanguageType(Enums.LanguageType.fr);

            _keysCount = Enum.GetNames(typeof(Enums.TranslationKeys)).Count();
        }

        [Fact]
        internal void TranslationSetup_ShouldBeReady()
        {
            Assert.NotNull(_en);
            Assert.NotNull(_fr);
        }

        private void ShouldGetTranslations(ResourceManager RM)
        {
            var resourceSet = RM.GetResourceSet(CultureInfo.InvariantCulture, true, true);
            var count = 0;

            foreach (DictionaryEntry o in resourceSet)
            {
                count++;
                var actual = (string)o.Key;
                var expected = actual.ToEnum<Enums.TranslationKeys>(ignoreCase: false)?.ToString();
                Assert.Equal(expected, actual);
            }

            Assert.Equal(count, _keysCount);
        }

        [Fact]
        internal void English_ShouldGetTranslations()
        {
            ShouldGetTranslations(new ResourceManager(_en));
        }

        [Fact]
        internal void French_ShouldGetTranslations()
        {
            ShouldGetTranslations(new ResourceManager(_fr));
        }

        [Fact]
        internal void Unsupported_ShouldNotGetTranslations()
        {
            RM = new ResourceManager(typeof(Translations.Languages));
            Assert.Throws<MissingManifestResourceException>(() => RM.GetResourceSet(CultureInfo.InvariantCulture, true, true));
        }
    }
}
