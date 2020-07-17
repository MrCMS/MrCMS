using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Website.Caching;

namespace MrCMS.Web.Admin.Services
{
    public class SetCacheExpiryOptions : BaseAssignWidgetAdminViewData<Widget>
    {
        public override void AssignViewData(Widget widget, ViewDataDictionary viewData)
        {
            viewData["cache-expiry-options"] = Enum.GetValues(typeof(CacheExpiryType)).Cast<CacheExpiryType>()
                .BuildSelectItemList(type => type.ToString().BreakUpString(),
                    type => type.ToString(),
                    emptyItem: null);
        }
    }
}