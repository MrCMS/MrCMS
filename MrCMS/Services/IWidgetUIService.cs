using System;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Entities.Widget;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IWidgetUIService
    {
        ActionResult GetContent(Controller controller, Widget widget, Func<HtmlHelper, MvcHtmlString> func);
        object GetModel(Widget widget);
        void SetAppDataToken(RouteData routeData, Widget widget);
    }
}