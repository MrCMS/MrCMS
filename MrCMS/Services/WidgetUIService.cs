using System;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Apps;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services.Caching;
using MrCMS.Website;

namespace MrCMS.Services
{
    public class WidgetUIService : IWidgetUIService
    {
        private readonly IGetWidgetCachingInfo _getWidgetCachingInfo;
        private readonly IHtmlCacheService _htmlCacheService;
        private readonly IWidgetModelService _widgetModelService;

        public WidgetUIService(IGetWidgetCachingInfo getWidgetCachingInfo,IHtmlCacheService htmlCacheService,IWidgetModelService widgetModelService)
        {
            _getWidgetCachingInfo = getWidgetCachingInfo;
            _htmlCacheService = htmlCacheService;
            _widgetModelService = widgetModelService;
        }

        public ActionResult GetContent(Controller controller, Widget widget, Func<IHtmlHelper, MvcHtmlString> func)
        {
            return _htmlCacheService.GetContent(controller, _getWidgetCachingInfo.Get(widget), func);
        }

        public object GetModel(Widget widget)
        {
            return _widgetModelService.GetModel(widget);
        }

        public void SetAppDataToken(RouteData routeData, Widget widget)
        {
            if (MrCMSApp.AppWidgets.ContainsKey(widget.Unproxy().GetType()))
                routeData.DataTokens["app"] = MrCMSApp.AppWidgets[widget.Unproxy().GetType()];
        }
    }
}