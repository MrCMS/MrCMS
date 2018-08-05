using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website;
using NHibernate;

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
            return helper.GetRequiredService<ISession>().Get<Webpage>(id)?.IsTypeCacheable() ?? false;
        }
    }
}