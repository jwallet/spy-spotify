using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EspionSpotify.Extensions
{
    public static class DictionaryExtensions
    {
        public static bool IncludesKey(this IDictionary<string, string> dictionary, string key)
        {
            return dictionary.Any() && dictionary.ContainsKey(key ?? string.Empty);
        }

        public static IDictionary<string, TValue> ToDictionary<TValue>(this object obj)
        {
            var json = JsonConvert.SerializeObject(obj ?? new object() { });
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, TValue>>(json);
            return dictionary;
        }

        public static KeyValuePair<TKey, TValue> ToKeyValuePair<TKey, TValue>(this object obj)
        {
            var json = JsonConvert.SerializeObject(obj ?? new object() { });
            if (!json.StartsWith("{") && !json.EndsWith("}")) return new KeyValuePair<TKey, TValue>();
            var pair = JsonConvert.DeserializeObject<KeyValuePair<TKey, TValue>>(json);
            return pair;
        }
    }
}
