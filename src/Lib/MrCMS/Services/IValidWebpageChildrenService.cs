using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IValidWebpageChildrenService
    {
        Task<IReadOnlyCollection<DocumentMetadata>> GetValidWebpageDocumentTypes(Webpage webpage,
            Func<DocumentMetadata, Task<bool>> predicate);

        Task<bool> AnyValidWebpageDocumentTypes(Webpage webpage, Func<DocumentMetadata, Task<bool>> predicate = null);
    }
}