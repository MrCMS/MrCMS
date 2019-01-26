using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IDocumentTagsUpdateService
    {
        void SetTags(string taglist, int id);
        void SetTags(string taglist, Document document);
        void SetTags(List<string> taglist, Document document);
    }
}