using System.Reflection;
using MrCMS.Entities.Widget;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public static class WidgetCachingExtensions
    {
        public static bool IsTypeCachable(this Widget widget)
        {
            return widget.GetType().GetCustomAttribute<OutputCacheableAttribute>() != null;
        }
    }
}