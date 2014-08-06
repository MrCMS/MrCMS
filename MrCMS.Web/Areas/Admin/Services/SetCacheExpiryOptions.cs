using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Website.Caching;

namespace MrCMS.Web.Areas.Admin.Services
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