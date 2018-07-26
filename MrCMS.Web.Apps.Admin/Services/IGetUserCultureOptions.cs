using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IGetUserCultureOptions
    {
        List<SelectListItem> Get();
    }
}