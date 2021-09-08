using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Multisite;

namespace MrCMS.Web.Admin.Services
{
    public interface IAdminSiteListService
    {
        Task<List<SelectListItem>> GetSiteOptions();
        Task<IList<Site>> GetSites();
    }
}