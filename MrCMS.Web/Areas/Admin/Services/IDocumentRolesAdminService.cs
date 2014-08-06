using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IDocumentRolesAdminService
    {
        void SetFrontEndRoles(string frontEndRoles, Webpage webpage);
    }
}