using System.Threading.Tasks;
using MrCMS.Entities.Documents;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface IDocumentVersionsAdminService
    {
        Task<VersionsModel> GetVersions(Document document, int page);

        Task<DocumentVersion> GetDocumentVersion(int id);
        Task<DocumentVersion> RevertToVersion(int id);
    }
}