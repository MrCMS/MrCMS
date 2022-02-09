namespace MrCMS.Website.Caching
{
    public class HighPriorityCacheManager : CacheManagerBase<IHighPriorityMemoryCache>, IHighPriorityCacheManager
    {
        public HighPriorityCacheManager(IHighPriorityMemoryCache cache) : base(cache)
        {
        }

        public void Clear()
        {
            Cache.Compact(100);
        }
    }
}