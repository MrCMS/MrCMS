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
        //public static T GetVersion<T>(this T doc, int id) where T : Document
        //{
        //    var documentVersion = doc.Versions.FirstOrDefault(version => version.Id == id);

        //    return documentVersion != null ? DeserializeVersion(documentVersion, doc) : null;
        //}


        private static List<VersionChange> GetVersionChanges(Document currentVersion, Document previousVersion)
        {
            var changes = new List<VersionChange>();

            if (previousVersion == null)
                return changes;

            var propertyInfos = currentVersion.GetType().GetVersionProperties();

            changes.AddRange(from propertyInfo in propertyInfos
                let oldValue = propertyInfo.GetValue(previousVersion, null)
                let currentValue = propertyInfo.GetValue(currentVersion, null)
                select new VersionChange
                {
                    Property =
                        propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true).
                            Any()
                            ? propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute),
                                true).OfType
                                <DisplayNameAttribute>().First().DisplayName
                            : propertyInfo.Name,
                    PreviousValue = oldValue,
                    CurrentValue = currentValue
                });
            return changes;
        }

        //public static bool AnyDifferencesFromCurrent(this DocumentVersion currentVersion)
        //{
        //    return GetComparisonToCurrent(currentVersion).Any(change => change.AnyChange);
        //}

        //public static List<VersionChange> GetComparisonToCurrent(this DocumentVersion currentVersion)
        //{
        //    var document = currentVersion.Document;
        //    var previousVersion = DeserializeVersion(currentVersion, document);

        //    return GetVersionChanges(document, previousVersion);
        //}

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