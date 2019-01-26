using System;
using MrCMS.Website.Caching;

namespace MrCMS.Models
{
    public class CachingInfo
    {
        public CachingInfo(bool shouldCache, string cacheKey, TimeSpan timeToCache, CacheExpiryType expiryType)
        {
            ShouldCache = shouldCache;
            CacheKey = cacheKey;
            ExpiryType = expiryType;
            TimeToCache = shouldCache ? timeToCache : TimeSpan.Zero;
        }

        public static readonly CachingInfo DoNotCache = new CachingInfo(false, "", TimeSpan.Zero, CacheExpiryType.Absolute);

        public bool ShouldCache { get; private set; }
        public string CacheKey { get; private set; }
        public TimeSpan TimeToCache { get; private set; }
        public CacheExpiryType ExpiryType { get; private set; }
    }
}