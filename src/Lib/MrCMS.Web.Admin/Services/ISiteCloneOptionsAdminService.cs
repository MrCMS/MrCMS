using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface ISiteCloneOptionsAdminService
    {
        List<SiteCloneOption> GetClonePartOptions();
        List<SelectListItem> GetOtherSiteOptions();
    }
}