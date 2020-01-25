using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
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