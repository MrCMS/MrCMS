using System.Reflection;
using MrCMS.Entities.Widget;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public static class WidgetCachingExtensions
    {
        public static bool IsTypeCacheable(this Widget widget)
        {
            return widget.GetType().GetCustomAttribute<WidgetOutputCacheableAttribute>() != null;
        }
        
        public static WidgetOutputCacheableAttribute GetWidgetOutputCacheable(this Widget widget)
        {
            return widget.GetType().GetCustomAttribute<WidgetOutputCacheableAttribute>() ?? new WidgetOutputCacheableAttribute();
        }
    }
}