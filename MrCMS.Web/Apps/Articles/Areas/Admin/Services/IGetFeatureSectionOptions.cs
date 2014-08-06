using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Services
{
    public interface IGetFeatureSectionOptions
    {
        List<SelectListItem> GetOptions();
    }
}