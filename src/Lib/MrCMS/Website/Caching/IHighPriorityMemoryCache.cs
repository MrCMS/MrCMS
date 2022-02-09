using Microsoft.Extensions.Caching.Memory;

namespace MrCMS.Website.Caching
{
    public interface IHighPriorityMemoryCache : IMemoryCache
    {
        void Compact(double percentage);
    }
}