using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Admin.Services;

namespace MrCMS.Web.Apps.Admin.Helpers
{
    public static class SetAdminViewDataExtensions
    {
        public static void SetAdminViewData<T>(this T webpage, Controller controller) where T : Webpage
        {
            controller.HttpContext.RequestServices.GetRequiredService<ISetWebpageAdminViewData>().SetViewData(webpage, controller.ViewData);
        }

        public static void SetViewData<T>(this T widget, Controller controller) where T : Widget
        {
            controller.HttpContext.RequestServices.GetRequiredService<ISetWidgetAdminViewData>().SetViewData(widget, controller.ViewData);
        }
    }
}