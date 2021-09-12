using System.Collections.Generic;

namespace RatesApi.Extensions
{
    public static class DictinoaryExtensions
    {
        public static Dictionary<string, T> CopyWithAddedPrefixToKeys<T>(this Dictionary<string, T> dictionary, string prefix)
        {
            var result = new Dictionary<string, T>();
            foreach(var item in dictionary)
            {
                result.Add(prefix + item.Key, item.Value);
            }
            return result;
        }
    }
}
