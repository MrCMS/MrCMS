using System.Web.Mvc;
using System.Web.Mvc.Html;
using MrCMS.Apps;
using MrCMS.Entities.Widget;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Helpers;
using MrCMS.Services.Caching;

namespace MrCMS.Website.Controllers
{
    public class WidgetController : MrCMSUIController
    {
        private readonly IWidgetModelService _service;
        private readonly IHtmlCacheService _htmlCacheService;

        public WidgetController(IWidgetModelService service, IHtmlCacheService htmlCacheService)
        {
            _service = service;
            _htmlCacheService = htmlCacheService;
        }

        public ActionResult Show(Widget widget)
        {
            CachingInfo cachingInfo = widget.GetCachingInfo();
            return _htmlCacheService.GetContent(this, cachingInfo,
                helper => helper.Action("Internal", "Widget", new { widget }));
        }

        public PartialViewResult Internal(Widget widget)
        {
            if (MrCMSApp.AppWidgets.ContainsKey(widget.Unproxy().GetType()))
                RouteData.DataTokens["app"] = MrCMSApp.AppWidgets[widget.Unproxy().GetType()];
            return PartialView(
                !string.IsNullOrWhiteSpace(widget.CustomLayout) ? widget.CustomLayout : widget.WidgetType,
                _service.GetModel(widget));
        }
    }
}