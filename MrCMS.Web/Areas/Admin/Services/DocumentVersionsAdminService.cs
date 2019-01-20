using System.Linq;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Areas.Admin.Models;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class DocumentVersionsAdminService : IDocumentVersionsAdminService
    {
        private readonly IRepository<DocumentVersion> _documentVersionRepository;
        private readonly IRepository<Document> _documentRepository;

        public DocumentVersionsAdminService(IRepository<DocumentVersion> documentVersionRepository,IRepository<Document> documentRepository)
        {
            _documentVersionRepository = documentVersionRepository;
            _documentRepository = documentRepository;
        }

        public VersionsModel GetVersions(Document document, int page)
        {
            IPagedList<DocumentVersion> versions = _documentVersionRepository.Query()
                .Where(version => version.Document.Id == document.Id)
                .OrderByDescending(version => version.CreatedOn)
                .Paged(page);

            return new VersionsModel(versions, document.Id);
        }

        public DocumentVersion GetDocumentVersion(int id)
        {
            return _documentVersionRepository.Get(id);
        }

        public void RevertToVersion(DocumentVersion documentVersion)
        {
            var currentVersion = documentVersion.Document.Unproxy();
            var previousVersion = currentVersion.GetVersion(documentVersion.Id);

            var versionProperties = currentVersion.GetType().GetVersionProperties();
            foreach (var versionProperty in versionProperties)
            {
                versionProperty.SetValue(currentVersion, versionProperty.GetValue(previousVersion, null), null);
            }
            _documentRepository.Update(currentVersion);
        }
    }
}