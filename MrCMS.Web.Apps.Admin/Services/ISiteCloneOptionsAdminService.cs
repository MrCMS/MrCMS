using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface ISiteCloneOptionsAdminService
    {
        List<SiteCloneOption> GetClonePartOptions();
        List<SelectListItem> GetOtherSiteOptions();
    }
}