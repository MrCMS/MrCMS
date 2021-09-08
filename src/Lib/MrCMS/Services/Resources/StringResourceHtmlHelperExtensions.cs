using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace MrCMS.Services.Resources
{
    public static class StringResourceHtmlHelperExtensions
    {
        public static async Task<string> Resource(this IHtmlHelper helper, string key, string defaultValue = null)
        {
            return await helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IStringResourceProvider>()
                .GetValue(key, defaultValue);
        }
    }
}