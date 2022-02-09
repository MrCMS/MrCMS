using Microsoft.AspNetCore.Mvc.Rendering;
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