using System.Threading.Tasks;
using MrCMS.Entities.Documents;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IDocumentVersionsAdminService
    {
        VersionsModel GetVersions(Document document, int page);

        DocumentVersion GetDocumentVersion(int id);
        Task<DocumentVersion> RevertToVersion(int id);
    }
}