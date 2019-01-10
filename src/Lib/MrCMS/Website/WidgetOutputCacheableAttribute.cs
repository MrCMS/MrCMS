using System;

namespace MrCMS.Website
{
    /// <summary>
    /// Marker attribute used to determine whether widgets can be output cached
    /// </summary>
    public class WidgetOutputCacheableAttribute : Attribute
    {
        public bool PerUser { get; set; }
        public bool PerPage { get; set; }
    }
}