using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Multisite;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IAdminSiteListService
    {
        List<SelectListItem> GetSiteOptions();
        IList<Site> GetSites();
    }
}