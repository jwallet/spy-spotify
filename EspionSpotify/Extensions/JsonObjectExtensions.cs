using System.Collections.Generic;
using Newtonsoft.Json;

namespace EspionSpotify.Extensions;

public static class JsonObjectExtensions
{ 
   public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this object obj)
   {
      var json = JsonConvert.SerializeObject(obj ?? new object());
      if (!json.StartsWith("{") && !json.EndsWith("}")) return new Dictionary<TKey, TValue>();
      var dictionary = JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(json);
      return dictionary;
   }

   public static KeyValuePair<TKey, TValue> ToKeyValuePair<TKey, TValue>(this object obj)
   {
      var json = JsonConvert.SerializeObject(obj ?? new object());
      if (!json.StartsWith("{") && !json.EndsWith("}")) return new KeyValuePair<TKey, TValue>();
      var pair = JsonConvert.DeserializeObject<KeyValuePair<TKey, TValue>>(json);
      return pair;
   }

   public static object[] ToArrayObject(this object obj)
   {
      var json = JsonConvert.SerializeObject(obj ?? new object());
      if (!json.StartsWith("[") && !json.EndsWith("]")) return null;
      return JsonConvert.DeserializeObject<object[]>(json);

   }
}