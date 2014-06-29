using System.Collections.Generic;
using Antlr.Runtime.Misc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IValidWebpageChildrenService
    {
        IEnumerable<DocumentMetadata> GetValidWebpageDocumentTypes(Webpage webpage, Func<DocumentMetadata, bool> predicate = null);
        bool AnyValidWebpageDocumentTypes(Webpage webpage, Func<DocumentMetadata, bool> predicate = null);
    }
}