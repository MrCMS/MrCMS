using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace MrCMS.Helpers
{
    public static class HtmlIoCExtensions
    {
        public static T GetRequiredService<T>(this IHtmlHelper html)
        {
            return html.ViewContext.HttpContext.RequestServices.GetRequiredService<T>();
        }

    }
}