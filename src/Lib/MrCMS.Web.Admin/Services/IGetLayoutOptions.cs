using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MrCMS.Web.Admin.Services
{
    public interface IGetLayoutOptions
    {
        List<SelectListItem> Get();
    }
}