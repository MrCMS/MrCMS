using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IGetLayoutOptions
    {
        List<SelectListItem> Get();
    }
}