using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;

namespace MrCMS.Web.Areas.Admin.Services
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