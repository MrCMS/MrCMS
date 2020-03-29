using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IDocumentRolesAdminService
    {
        IList<FrontEndAllowedRole> GetFrontEndRoles(Webpage webpage, string frontEndRoles, bool inheritFromParent);
    }
}