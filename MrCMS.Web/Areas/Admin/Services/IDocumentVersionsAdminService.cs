using MrCMS.Entities.Documents;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IDocumentVersionsAdminService
    {
        VersionsModel GetVersions(Document document, int page);

        DocumentVersion GetDocumentVersion(int id);
        void RevertToVersion(DocumentVersion documentVersion);
    }
}