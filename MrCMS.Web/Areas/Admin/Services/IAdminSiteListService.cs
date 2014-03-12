using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IAdminSiteListService
    {
        List<SelectListItem> GetSiteOptions();
    }
}