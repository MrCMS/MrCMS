using System.Collections.Generic;
using MrCMS.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IAdminNavLinksService
    {
        IEnumerable<IAdminMenuItem> GetNavLinks();
    }
}