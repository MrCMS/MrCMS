using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Helpers
{
    public static class TempDataExtensions
    {
        public static void Set<T>(this ITempDataDictionary tempData, T model, params object[] context) where T : class
        {
            Set(tempData, typeof(T), model, context);
        }
        public static void Set(this ITempDataDictionary tempData, Type type, object model, params object[] context)
        {
            if (tempData == null)
            {
                return;
            }

            var key = type.FullName;
            var serializableObjects = context.Where(x => x != null);
            if (serializableObjects.Any())
            {
                key += "." + string.Join(".", serializableObjects);
            }

            tempData[key] = JsonConvert.SerializeObject(model, SerializerSettings);
        }

        public static object Get(this ITempDataDictionary tempData, Type type, params object[] context)
        {
            if (tempData == null)
            {
                return type.GetDefaultValue();
            }

            var key = type.FullName;
            var serializableObjects = context.Where(x => x != null);
            if (serializableObjects.Any())
            {
                key += "." + string.Join(".", serializableObjects);
            }

            if (!tempData.ContainsKey(key))
            {
                return type.GetDefaultValue();
            }

            return JsonConvert.DeserializeObject(tempData[key].ToString(), type);
        }

        public static T Get<T>(this ITempDataDictionary tempData, params object[] context) where T : class
        {
            return Get(tempData, typeof(T), context) as T;
        }

        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new WritablePropertiesOnlyResolver()
        };
        class WritablePropertiesOnlyResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
                return props.Where(p => p.Writable).ToList();
            }
        }
    }
}