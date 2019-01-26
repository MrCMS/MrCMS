using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace MrCMS.Website.Caching
{
    public interface IClearableInMemoryCache : IEnumerable<KeyValuePair<object, object>>, IMemoryCache
    {
        /// <summary>
        /// Clears all cache entries.
        /// </summary>
        void Clear(string prefix = null);
    }
}