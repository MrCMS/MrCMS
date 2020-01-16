using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IDocumentRolesAdminService
    {
        IList<FrontEndAllowedRole> GetFrontEndRoles(Webpage webpage, string frontEndRoles, bool inheritFromParent);
    }
}