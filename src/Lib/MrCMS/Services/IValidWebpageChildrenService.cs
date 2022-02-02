using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IValidWebpageChildrenService
    {
        Task<IReadOnlyCollection<WebpageMetadata>> GetValidWebpageTypes(Webpage webpage,
            Func<WebpageMetadata, Task<bool>> predicate);

        Task<bool> AnyValidWebpageTypes(Webpage webpage, Func<WebpageMetadata, Task<bool>> predicate = null);
    }
}