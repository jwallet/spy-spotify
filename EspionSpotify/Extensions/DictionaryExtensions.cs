using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace EspionSpotify.Extensions
{
    public static class DictionaryExtensions
    {
        public static bool IncludesKey(this IDictionary<string, string> dictionary, string key)
        {
            return dictionary.Any() && dictionary.ContainsKey(key ?? string.Empty);
        }
    }
}