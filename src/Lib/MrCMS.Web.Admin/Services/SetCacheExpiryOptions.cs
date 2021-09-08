using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Website.Caching;

namespace MrCMS.Web.Admin.Services
{
    public class SetCacheExpiryOptions : BaseAssignWidgetAdminViewData<Widget>
    {
        public override Task AssignViewData(Widget widget, ViewDataDictionary viewData)
        {
            viewData["cache-expiry-options"] = Enum.GetValues(typeof(CacheExpiryType)).Cast<CacheExpiryType>()
                .BuildSelectItemList(type => type.ToString().BreakUpString(),
                    type => type.ToString(),
                    emptyItem: null);
            return Task.CompletedTask;
        }
    }
}