using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities;
using MrCMS.Entities.Documents;
using MrCMS.Models;
using MrCMS.Paging;
using Newtonsoft.Json;

namespace MrCMS.Helpers
{
    public static class DocumentExtensions
    {
        public static void SetParent(this Document document, Document parent)
        {
            document.Parent = parent;
            if (parent != null && !parent.Children.Contains(document))
                parent.Children.Add(document);
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

            var propertyInfos =
                currentVersion.GetType().GetProperties()
                    .Where(info =>
                           info.CanWrite && !info.PropertyType.IsGenericType && !typeof(SiteEntity).IsAssignableFrom(info.PropertyType) &&
                           info.DeclaringType != typeof(SiteEntity))
                    .ToList();

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

        public static List<VersionChange> GetComparisonToCurrent(this DocumentVersion currentVersion)
        {
            var document = currentVersion.Document.Unproxy();
            var previousVersion = DeserializeVersion(currentVersion, document);

            return GetVersionChanges(document, previousVersion);
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