using System.Threading.Tasks;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface IWebpageVersionsAdminService
    {
        Task<VersionsModel> GetVersions(Webpage webpage, int page);

        Task<WebpageVersion> GetDocumentVersion(int id);
        Task<WebpageVersion> RevertToVersion(int id);
    }
}