using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MrCMS.Website.Caching
{
    public class StandardPriorityMemoryCache : MemoryCache, IStandardPriorityMemoryCache
    {
        public StandardPriorityMemoryCache(IOptions<MemoryCacheOptions> optionsAccessor) : base(optionsAccessor)
        {
        }

        public StandardPriorityMemoryCache(IOptions<MemoryCacheOptions> optionsAccessor, ILoggerFactory loggerFactory) :
            base(optionsAccessor, loggerFactory)
        {
        }
    }
}