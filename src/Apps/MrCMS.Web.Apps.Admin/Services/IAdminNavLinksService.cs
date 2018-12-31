using System.Collections.Generic;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IAdminNavLinksService
    {
        IEnumerable<IAdminMenuItem> GetNavLinks();
    }
}