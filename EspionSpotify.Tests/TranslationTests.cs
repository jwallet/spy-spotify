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
        private readonly Type _cs;
        private readonly Type _de;
        private readonly Type _en;
        private readonly Type _es;
        private readonly Type _fr;
        private readonly Type _it;
        private readonly Type _ja;
        private readonly Type _nl;
        private readonly Type _pl;
        private readonly Type _pt;
        private readonly Type _ru;
        private readonly Type _tr;
        private static ResourceManager RM;
        private readonly int _keysCount;

        public TranslationTests()
        {
            _cs = Translations.Languages.GetResourcesManagerLanguageType(Enums.LanguageType.cs);
            _de = Translations.Languages.GetResourcesManagerLanguageType(Enums.LanguageType.de);
            _en = Translations.Languages.GetResourcesManagerLanguageType(Enums.LanguageType.en);
            _es = Translations.Languages.GetResourcesManagerLanguageType(Enums.LanguageType.es);
            _fr = Translations.Languages.GetResourcesManagerLanguageType(Enums.LanguageType.fr);
            _it = Translations.Languages.GetResourcesManagerLanguageType(Enums.LanguageType.it);
            _ja = Translations.Languages.GetResourcesManagerLanguageType(Enums.LanguageType.ja);
            _nl = Translations.Languages.GetResourcesManagerLanguageType(Enums.LanguageType.nl);
            _pl = Translations.Languages.GetResourcesManagerLanguageType(Enums.LanguageType.pl);
            _pt = Translations.Languages.GetResourcesManagerLanguageType(Enums.LanguageType.pt);
            _ru = Translations.Languages.GetResourcesManagerLanguageType(Enums.LanguageType.ru);
            _tr = Translations.Languages.GetResourcesManagerLanguageType(Enums.LanguageType.tr);

            _keysCount = Enum.GetNames(typeof(Enums.TranslationKeys)).Count();
        }

        [Fact]
        internal void TranslationSetup_ShouldBeReady()
        {
            Assert.NotNull(_cs);
            Assert.NotNull(_de);
            Assert.NotNull(_en);
            Assert.NotNull(_es);
            Assert.NotNull(_fr);
            Assert.NotNull(_it);
            Assert.NotNull(_ja);
            Assert.NotNull(_nl);
            Assert.NotNull(_pl);
            Assert.NotNull(_pt);
            Assert.NotNull(_ru);
            Assert.NotNull(_tr);
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
        internal void Czech_ShouldGetTranslations()
        {
            ShouldGetTranslations(new ResourceManager(_cs));
        }

        [Fact]
        internal void German_ShouldGetTranslations()
        {
            ShouldGetTranslations(new ResourceManager(_de));
        }

        [Fact]
        internal void English_ShouldGetTranslations()
        {
            ShouldGetTranslations(new ResourceManager(_en));
        }

        [Fact]
        internal void Spanish_ShouldGetTranslations()
        {
            ShouldGetTranslations(new ResourceManager(_es));
        }

        [Fact]
        internal void French_ShouldGetTranslations()
        {
            ShouldGetTranslations(new ResourceManager(_fr));
        }

        [Fact]
        internal void Italian_ShouldGetTranslations()
        {
            ShouldGetTranslations(new ResourceManager(_it));
        }

        [Fact]
        internal void Japan_ShouldGetTranslations()
        {
            ShouldGetTranslations(new ResourceManager(_ja));
        }

        [Fact]
        internal void Dutch_ShouldGetTranslations()
        {
            ShouldGetTranslations(new ResourceManager(_nl));
        }

        [Fact]
        internal void Polish_ShouldGetTranslations()
        {
            ShouldGetTranslations(new ResourceManager(_pl));
        }

        [Fact]
        internal void Portuguese_ShouldGetTranslations()
        {
            ShouldGetTranslations(new ResourceManager(_pt));
        }

        [Fact]
        internal void Russian_ShouldGetTranslations()
        {
            ShouldGetTranslations(new ResourceManager(_ru));
        }

        [Fact]
        internal void Turkish_ShouldGetTranslations()
        {
            ShouldGetTranslations(new ResourceManager(_tr));
        }

        [Fact]
        internal void Unsupported_ShouldNotGetTranslations()
        {
            RM = new ResourceManager(typeof(Translations.Languages));
            Assert.Throws<MissingManifestResourceException>(() => RM.GetResourceSet(CultureInfo.InvariantCulture, true, true));
        }
    }
}
