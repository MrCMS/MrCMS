using Microsoft.Extensions.Caching.Memory;

namespace MrCMS.Website.Caching
{
    public interface IStandardPriorityMemoryCache : IMemoryCache
    {
        void Compact(double percentage);
    }
}