using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Admin.Infrastructure.Services
{
    public interface IGetDocumentTagsService
    {
        ISet<Tag> GetTags(string tagList);
        IList<DocumentTag> GetDocumentTags(Document document, string tagList);
    }
}