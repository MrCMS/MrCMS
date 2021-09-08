using System.Collections.Generic;
using MrCMS.Entities.People;

namespace MrCMS.Web.Admin.Services
{
    public interface IDocumentRolesAdminService
    {
        ISet<UserRole> GetFrontEndRoles(string frontEndRoles);
    }
}