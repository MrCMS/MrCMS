using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using MrCMS.Entities;
using MrCMS.Entities.Documents;
using MrCMS.Models;
using Newtonsoft.Json;

namespace MrCMS.Helpers
{
    public static class DocumentVersionExtensions
    {
        public static List<PropertyInfo> GetVersionProperties(this Type type)
        {
            var ignorePropertyNames = new[]
            {
                "UpdatedOn", "Id", "CreatedOn"
            };

            return type.GetProperties().Where(
                info =>
                    info.CanWrite &&
                    !typeof(SystemEntity).IsAssignableFrom(info.PropertyType) &&
                    (!info.PropertyType.IsGenericType ||
                     (info.PropertyType.IsGenericType && info.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    &&
                    !ignorePropertyNames.Contains(info.Name)).ToList();
        }
    }
}