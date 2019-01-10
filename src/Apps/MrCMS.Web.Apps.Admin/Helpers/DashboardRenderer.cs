using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Admin.Infrastructure.Dashboard;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MrCMS.Web.Apps.Admin.Helpers
{
    public static class DashboardRenderer
    {
        public static async Task<IHtmlContent> RenderDashboardArea(this IViewComponentHelper componentHelper, DashboardArea dashboardArea)
        {
            var components = TypeHelper.GetAllConcreteTypesAssignableFrom<ViewComponent>()
                .Select(type => new
                {
                    type,
                    attribute = type.GetCustomAttribute<DashboardAreaAttribute>()
                })
                .Where(x => x.attribute != null && x.attribute.Area == dashboardArea)
                .OrderBy(x => x.attribute.Order)
                .Select(x => x.type)
                .ToList();

            if (!components.Any())
            {
                return HtmlString.Empty;
            }

            IHtmlContentBuilder htmlContentBuilder = new HtmlContentBuilder();

            foreach (var component in components)
            {
                htmlContentBuilder = htmlContentBuilder.AppendHtml(await componentHelper.InvokeAsync(component));
            }

            return htmlContentBuilder;
        }
    }
}