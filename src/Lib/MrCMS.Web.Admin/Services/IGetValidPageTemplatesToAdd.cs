using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Admin.Services
{
    public interface IGetValidPageTemplatesToAdd
    {
        List<PageTemplate> Get(IEnumerable<WebpageMetadata> validWebpageDocumentTypes);
    }
}