using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Helpers
{
    public static class DashboardRenderer
    {
        public static IHtmlContent RenderDashboardArea(this IHtmlHelper htmlHelper, DashboardArea dashboardArea)
        {
            //var actionMethodsWithAttribute =
            //    MrCMSControllerFactory.GetActionMethodsWithAttribute<DashboardAreaAction>()
            //        .Where(info => info.Attribute.DashboardArea == dashboardArea);

            //return actionMethodsWithAttribute.OrderBy(info => info.Attribute.Order)
            //    .Aggregate(MvcHtmlString.Empty,
            //        (current, actionMethodInfo) =>
            //            current.Concat(htmlHelper.Action(actionMethodInfo.Descriptor.ActionName,
            //                actionMethodInfo.Descriptor.ControllerDescriptor.ControllerName)));
            // TODO: needs refactoring to view components
            return HtmlString.Empty;
        }

        //public static MvcHtmlString Concat(this MvcHtmlString first, params MvcHtmlString[] strings)
        //{
        //    return MvcHtmlString.Create(first + string.Concat(strings.Select(s => s.ToString())));
        //}
    }
}