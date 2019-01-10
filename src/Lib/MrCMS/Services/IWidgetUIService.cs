using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using MrCMS.Entities.Widget;

namespace MrCMS.Services
{
    public interface IWidgetUIService
    {
        ActionResult GetContent(Controller controller, Widget widget, Func<IHtmlHelper, IHtmlContent> func);
        (Widget Widget, object Model) GetModel(int id);
        object GetModel(Widget widget);
        void SetAppDataToken(RouteData routeData, Widget widget);
    }
}