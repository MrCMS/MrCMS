using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MrCMS.Website;

namespace MrCMS.Web.Areas.Admin.Helpers
{
    public static class DashboardRenderer
    {
        public static MvcHtmlString RenderDashboardArea(this HtmlHelper htmlHelper, DashboardArea dashboardArea)
        {
            var actionMethodsWithAttribute =
                MrCMSControllerFactory.GetActionMethodsWithAttribute<DashboardAreaAction>()
                    .Where(info => info.Attribute.DashboardArea == dashboardArea);

            return actionMethodsWithAttribute.OrderBy(info => info.Attribute.Order)
                .Aggregate(MvcHtmlString.Empty,
                    (current, actionMethodInfo) =>
                        current.Concat(htmlHelper.Action(actionMethodInfo.Descriptor.ActionName,
                            actionMethodInfo.Descriptor.ControllerDescriptor.ControllerName)));
        }

        public static MvcHtmlString Concat(this MvcHtmlString first, params MvcHtmlString[] strings)
        {
            return MvcHtmlString.Create(first + string.Concat(strings.Select(s => s.ToString())));
        }
    }

    public class DashboardAreaAction : ActionFilterAttribute
    {
        public DashboardArea DashboardArea { get; set; }
    }

    public enum DashboardArea
    {
        Top,
        LeftColumn,
        RightColumn
    }
}