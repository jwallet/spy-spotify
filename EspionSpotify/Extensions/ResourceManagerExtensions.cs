using System.Resources;
using EspionSpotify.Enums;

namespace EspionSpotify.Extensions
{
    public static class ResourceManagerExtensions
    {
        public static string GetString(this ResourceManager rm, TranslationKeys key)
        {
            return rm.GetString(key.ToString());
        }
    }
}