using System;
using MrCMS.Website.Caching;

namespace MrCMS.Website
{
    /// <inheritdoc />
    /// <summary>
    /// Marker attribute used to determine whether webpage can be output cached
    /// </summary>
    public class WebpageOutputCacheableAttribute : Attribute
    {
        public CacheExpiryType ExpiryType { get; }
        public int Length { get; }

        public WebpageOutputCacheableAttribute(CacheExpiryType expiryType, int length)
        {
            ExpiryType = expiryType;
            Length = length;
        }
    }
}