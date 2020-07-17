using MrCMS.Entities.Documents;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface IDocumentVersionsAdminService
    {
        VersionsModel GetVersions(Document document, int page);

        DocumentVersion GetDocumentVersion(int id);
        DocumentVersion RevertToVersion(int id);
    }
}