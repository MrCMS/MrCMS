using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class GetUserCultureOptions : IGetUserCultureOptions
    {
        public List<SelectListItem> Get()
        {
            return CultureInfo.GetCultures(CultureTypes.AllCultures).OrderBy(info => info.DisplayName)
                .BuildSelectItemList(info => info.DisplayName, info => info.Name,
                    emptyItemText: "System Default");

        }
    }
}