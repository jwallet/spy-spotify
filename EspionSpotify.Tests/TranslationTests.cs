using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using EspionSpotify;
using EspionSpotify.Extensions;
using Xunit;

namespace EspionSpotify.Tests
{
    public class TranslationTests
    {
        private readonly Type _en;
        private readonly Type _fr;
        private readonly Type _nl;
        private static ResourceManager RM;
        private readonly int _keysCount;

        public TranslationTests()
        {
            _en = Translations.Languages.getResourcesManagerLanguageType(Enums.LanguageType.en);
            _fr = Translations.Languages.getResourcesManagerLanguageType(Enums.LanguageType.fr);
            _nl = Translations.Languages.getResourcesManagerLanguageType(Enums.LanguageType.nl);
            _keysCount = Enum.GetNames(typeof(Enums.TranslationKeys)).Count();
        }

        [Fact]
        internal void TranslationSetup_ShouldBeReady()
        {
            Assert.NotNull(_en);
            Assert.NotNull(_fr);
            //Assert.NotNull(_nl);
        }

        [Fact]
        internal void English_ShouldGetTranslations()
        {
            RM = new ResourceManager(_en);
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
        internal void French_ShouldGetTranslations()
        {
            RM = new ResourceManager(_fr);
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

        [Fact(Skip = "NL unsupported yet")]
        internal void Dutch_ShouldGetTranslations()
        {
            RM = new ResourceManager(_nl);
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
        internal void Unsupported_ShouldNotGetTranslations()
        {
            RM = new ResourceManager(typeof(Translations.Languages));
            Assert.Throws<MissingManifestResourceException>(() => RM.GetResourceSet(CultureInfo.InvariantCulture, true, true));
        }
    }
}
