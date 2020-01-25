using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Models;
using Newtonsoft.Json;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class DocumentVersionsAdminService : IDocumentVersionsAdminService
    {
        private readonly IRepository<DocumentVersion> _documentVersionRepository;
        private readonly IRepository<Document> _documentRepository;

        public DocumentVersionsAdminService(IRepository<DocumentVersion> documentVersionRepository, IRepository<Document> documentRepository)
        {
            _documentVersionRepository = documentVersionRepository;
            _documentRepository = documentRepository;
        }

        public VersionsModel GetVersions(Document document, int page)
        {
            IPagedList<DocumentVersion> versions = _documentVersionRepository.Query()
                .Where(version => version.Document.Id == document.Id)
                .OrderByDescending(version => version.CreatedOn)
                .ToPagedList(page);

            return new VersionsModel(versions, document.Id);
        }

        public DocumentVersion GetDocumentVersion(int id)
        {
            return _documentVersionRepository.LoadSync(id, version => version.Document);
        }

        public async Task<DocumentVersion> RevertToVersion(int id)
        {
            var documentVersion = GetDocumentVersion(id);

            var currentVersion = await _documentRepository.Load(documentVersion.DocumentId);
            var previousVersion = await _documentVersionRepository.Load(documentVersion.Id);

            var versionProperties = currentVersion.GetType().GetVersionProperties();
            foreach (var versionProperty in versionProperties)
            {
                versionProperty.SetValue(currentVersion, versionProperty.GetValue(previousVersion, null), null);
            }
            await _documentRepository.Update(currentVersion);
            return documentVersion;
        }

        public async Task<bool> AnyDifferencesFromCurrent(DocumentVersion version)
        {
            return (await GetComparisonToCurrent(version)).Any(change => change.AnyChange);
        }

        public async Task<IEnumerable<VersionChange>> GetComparisonToCurrent(DocumentVersion version)
        {
            var document = await _documentRepository.Load(version.DocumentId);
            var previousVersion = DeserializeVersion(version, document);

            return GetVersionChanges(document, previousVersion);
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
    }
}