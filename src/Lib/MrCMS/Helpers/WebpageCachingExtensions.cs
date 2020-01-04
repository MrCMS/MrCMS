using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public static class WebpageCachingExtensions
    {
        public static bool IsTypeCacheable(this Webpage webpage)
        {
            return webpage.GetType().GetCustomAttribute<WebpageOutputCacheableAttribute>() != null;
        }
        public static bool IsWebpageCacheable(this IHtmlHelper helper, int id)
        {
            return helper.GetRequiredService<IRepository<Webpage>>().GetDataSync(id)?.IsTypeCacheable() ?? false;
        }
    }
}