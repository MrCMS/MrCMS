using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IGetLayoutOptions
    {
        List<SelectListItem> Get();
    }
}