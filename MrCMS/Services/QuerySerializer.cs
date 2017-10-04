using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using MrCMS.Helpers;

namespace MrCMS.Services
{
    public class QuerySerializer : IQuerySerializer
    {
        public static IDictionary<string, object> GetRouteDictionary(object obj)
        {
            var values = GetPairs(obj);

            IDictionary<string, object> dictionary = new Dictionary<string, object>();

            foreach (var pair in values)
                dictionary.Add(pair.Key, pair.Value);

            return dictionary;
        }

        public IDictionary<string, object> GetRoutingData(object obj)
        {
            return GetRouteDictionary(obj);
        }

        public string AppendToUrl(string url, IDictionary<string, object> routingData)
        {
            if (routingData == null || !routingData.Keys.Any())
                return url;
            return $"{url}?{Serialize(routingData)}";
        }

        private string Serialize(IDictionary<string, object> data)
        {
            return string.Join("&",
                data.Keys.Select(x => $"{WebUtility.UrlEncode(x)}={WebUtility.UrlEncode(data[x].ToString())}"));
        }


        private static IEnumerable<KeyValuePair<string, object>> GetPairs(object obj)
        {
            if (obj == null)
                yield break;

            foreach (var pair in GetKeyValuePairs(obj))
                yield return pair;
        }

        private static IEnumerable<KeyValuePair<string, object>> GetKeyValuePairs(object query)
        {
            var properties = query.GetType().GetProperties().Where(x => x.CanRead).ToList();

            return properties.SelectMany(info => GetKeyValuePairs(info, query));
        }

        private static IEnumerable<KeyValuePair<string, object>> GetKeyValuePairs(PropertyInfo arg, object query)
        {
            var value = arg.GetValue(query, null);
            if (arg.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(arg.PropertyType))
            {
                if (value is IEnumerable enumerable)
                    foreach (var item in enumerable.OfType<object>().Select((o, i) => new {o, i}))
                        yield return new KeyValuePair<string, object>($"{arg.Name}[{item.i}]", item.o);
            }
            else
            {
                if (ShouldBeMapped(value, arg.PropertyType))
                    yield return new KeyValuePair<string, object>(arg.Name, value);
            }
        }

        private static bool ShouldBeMapped(object value, Type propertyType)
        {
            if (value is string s)
                return !string.IsNullOrWhiteSpace(s);

            return !Equals(value, propertyType.GetDefaultValue());
        }
    }
}