using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using MrCMS.Entities;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website;
using Newtonsoft.Json;
using NHibernate;

namespace MrCMS.Helpers
{
    public static class DocumentExtensions
    {
        public static bool CanDelete(this Document document)
        {
            return MrCMSApplication.Get<ISession>()
                .QueryOver<Document>()
                .Where(doc => doc.Parent != null && doc.Parent.Id == document.Id)
                .Cacheable()
                .Any();
        }
        public static int FormPostingsCount(this Webpage webpage)
        {
            return MrCMSApplication.Get<ISession>()
                .QueryOver<FormPosting>()
                .Where(posting => posting.Webpage != null && posting.Webpage.Id == webpage.Id)
                .Cacheable()
                .RowCount();
        }


        public static void SetParent(this Document document, Document parent)
        {
            var existingParent = document.Parent;
            if (existingParent == parent)
                return;
            document.Parent = parent;
            if (parent != null && !parent.Children.Contains(document))
                parent.Children.Add(document);
            if (existingParent != null)
                existingParent.Children.Remove(document);
        }

        public static T GetVersion<T>(this T doc, int id) where T : Document
        {
            var documentVersion = doc.Versions.FirstOrDefault(version => version.Id == id).Unproxy();

            return documentVersion != null ? DeserializeVersion(documentVersion, doc) : null;
        }

        private static T DeserializeVersion<T>(DocumentVersion version, T doc) where T : Document
        {
            return JsonConvert.DeserializeObject(version.Data, doc.GetType()) as T;
        }

        public static List<VersionChange> GetComparison(this Document currentVersion, int verisonId)
        {
            var previousVersion = currentVersion.GetVersion(verisonId);

            return GetVersionChanges(currentVersion, previousVersion);
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
                                                propertyInfo.GetCustomAttributes(typeof (DisplayNameAttribute), true).
                                                    Any()
                                                    ? propertyInfo.GetCustomAttributes(typeof (DisplayNameAttribute),
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
                !typeof (SystemEntity).IsAssignableFrom(info.PropertyType) &&
                (!info.PropertyType.IsGenericType ||
                 (info.PropertyType.IsGenericType && info.PropertyType.GetGenericTypeDefinition() == typeof (Nullable<>)))
                &&
                !ignorePropertyNames.Contains(info.Name)).ToList();
        }
    }

    public class VersionChange
    {
        public string Property { get; set; }

        public object PreviousValue { get; set; }

        public object CurrentValue { get; set; }

        public bool AnyChange
        {
            get { return !Equals(PreviousValue, CurrentValue); }
        }
    }
}