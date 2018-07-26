using System.Collections.Generic;
using MrCMS.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IAdminNavLinksService
    {
        IEnumerable<IAdminMenuItem> GetNavLinks();
    }
}