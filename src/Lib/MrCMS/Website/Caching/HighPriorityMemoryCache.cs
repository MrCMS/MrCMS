using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MrCMS.Website.Caching
{
    public class HighPriorityMemoryCache : MemoryCache, IHighPriorityMemoryCache
    {
        public HighPriorityMemoryCache(IOptions<MemoryCacheOptions> optionsAccessor) : base(optionsAccessor)
        {
        }

        public HighPriorityMemoryCache(IOptions<MemoryCacheOptions> optionsAccessor, ILoggerFactory loggerFactory) :
            base(optionsAccessor, loggerFactory)
        {
        }
    }
}