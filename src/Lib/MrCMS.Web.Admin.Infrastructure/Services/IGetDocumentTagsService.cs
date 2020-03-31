using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Admin.Infrastructure.Services
{
    public interface IGetDocumentTagsService
    {
        ISet<Tag> GetTags(string tagList);
        Task<IList<DocumentTag>> GetDocumentTags(Document document, string tagList);
    }
}