using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IGetValidPageTemplatesToAdd
    {
        Task<List<PageTemplate>> Get(IEnumerable<DocumentMetadata> validWebpageDocumentTypes);
    }
}