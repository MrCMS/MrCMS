using MrCMS.Entities.Documents;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IDocumentTagsAdminService
    {
        void SetTags(string taglist, Document document);
    }
}