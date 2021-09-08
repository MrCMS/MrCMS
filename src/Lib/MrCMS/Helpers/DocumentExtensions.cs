using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;
using Newtonsoft.Json;
using NHibernate;

namespace MrCMS.Helpers
{
    public static class DocumentExtensions
    {
        public static bool CanDeleteDocument(this IHtmlHelper helper, int id)
        {
            return !helper.AnyChildren(id);
        }

        public static bool AnyChildren(this IHtmlHelper helper, int id)
        {
            var document = helper.GetRequiredService<ISession>().Get<Document>(id);
            if (document == null)
                return false;
            return AnyChildren(helper, document);
        }

        private static bool AnyChildren(this IHtmlHelper helper, Document document)
        {
            return helper.GetRequiredService<ISession>()
                .QueryOver<Document>()
                .Where(doc => doc.Parent != null && doc.Parent.Id == document.Id)
                .Cacheable()
                .Any();
        }

        public static bool CanDelete<T>(this IHtmlHelper<T> helper) where T : Document
        {
            return !helper.AnyChildren(helper.ViewData.Model);
        }

        public static bool AnyChildren<T>(this IHtmlHelper<T> helper) where T : Document
        {
            return helper.AnyChildren(helper.ViewData.Model);
        }

        public static string GetAdminController(this Document document)
        {
            return document is Layout ? "Layout" : document is MediaCategory ? "MediaCategory" : "Webpage";
        }

        public static T GetVersion<T>(this T doc, int id) where T : Document
        {
            var documentVersion = doc.Versions.FirstOrDefault(version => version.Id == id).Unproxy();

            return documentVersion != null ? DeserializeVersion(documentVersion, doc) : null;
        }

        private static T DeserializeVersion<T>(DocumentVersion version, T doc) where T : Document
        {
            // use null handling ignore so that properties that didn't exist in previous versions are defaulted
            return JsonConvert.DeserializeObject(version.Data, doc.GetType(), new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }) as T;
        }

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

        public static bool AnyDifferencesFromCurrent(this DocumentVersion currentVersion)
        {
            return GetComparisonToCurrent(currentVersion).Any(change => change.AnyChange);
        }

        public static List<VersionChange> GetComparisonToCurrent(this DocumentVersion currentVersion)
        {
            var document = currentVersion.Document.Unproxy();
            var previousVersion = DeserializeVersion(currentVersion, document);

            return GetVersionChanges(document, previousVersion);
        }

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