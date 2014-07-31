using System;

namespace MrCMS.Models
{
    public class CachingInfo
    {
        public CachingInfo(bool shouldCache, string cacheKey, TimeSpan timeToCache)
        {
            ShouldCache = shouldCache;
            CacheKey = cacheKey;
            TimeToCache = shouldCache ? timeToCache : TimeSpan.Zero;
        }

        public static readonly CachingInfo DoNotCache = new CachingInfo(false, "", TimeSpan.Zero);

        public bool ShouldCache { get; private set; }
        public string CacheKey { get; private set; }
        public TimeSpan TimeToCache { get; private set; }
    }
}