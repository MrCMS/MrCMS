using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents;
using MrCMS.Models;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IDocumentVersionsAdminService
    {
        VersionsModel GetVersions(Document document, int page);

        DocumentVersion GetDocumentVersion(int id);
        Task<DocumentVersion> RevertToVersion(int id);
        Task<bool> AnyDifferencesFromCurrent(DocumentVersion version);
        Task<IEnumerable<VersionChange>> GetComparisonToCurrent(DocumentVersion version);
    }
}