using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IDocumentRolesAdminService
    {
        void SetFrontEndRoles(string frontEndRoles, Webpage webpage);
    }
}