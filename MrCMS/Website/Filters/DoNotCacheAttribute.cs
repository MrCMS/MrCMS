using System;

namespace MrCMS.Website.Filters
{
    public class DoNotCacheAttribute : Attribute
    {
        public const string TempDataKey = "do-not-cache";
    }
}