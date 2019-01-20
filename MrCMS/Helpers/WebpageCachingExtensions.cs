using System.Reflection;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public static class WebpageCachingExtensions
    {
        public static bool IsTypeCachable(this Webpage webpage)
        {
            return webpage.GetType().GetCustomAttribute<WebpageOutputCacheableAttribute>() != null;
        }
    }
}