using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace MrCMS.Services.Resources
{
    public static class StringResourceHtmlHelperExtensions
    {
        public static string Resource(this IHtmlHelper helper, string key, string defaultValue = null)
        {
            return helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IStringResourceProvider>()
                .GetValue(key, defaultValue);
        }
    }
}