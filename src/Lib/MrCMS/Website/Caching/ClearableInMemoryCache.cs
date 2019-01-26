using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Website.Caching
{
    public class ClearableInMemoryCache : IClearableInMemoryCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ConcurrentDictionary<string, ICacheEntry> _cacheEntries = new ConcurrentDictionary<string, ICacheEntry>();

        public ClearableInMemoryCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void Dispose()
        {
            _memoryCache.Dispose();
        }

        private void PostEvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            if (reason != EvictionReason.Replaced)
            {
                _cacheEntries.TryRemove(key.ToString(), out _);
            }
        }

        /// <inheritdoc cref="IMemoryCache.TryGetValue"/>
        public bool TryGetValue(object key, out object value)
        {
            return _memoryCache.TryGetValue(key, out value);
        }

        /// <summary>
        /// Create or overwrite an entry in the cache and add key to Dictionary.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        /// <returns>The newly created <see cref="T:Microsoft.Extensions.Caching.Memory.ICacheEntry" /> instance.</returns>
        public ICacheEntry CreateEntry(object key)
        {
            var fullKey = key.ToString();
            var entry = _memoryCache.CreateEntry(fullKey);
            entry.RegisterPostEvictionCallback(PostEvictionCallback);
            _cacheEntries.AddOrUpdate(fullKey, entry, (o, cacheEntry) =>
            {
                cacheEntry.Value = entry;
                return cacheEntry;
            });
            return entry;
        }

        /// <inheritdoc cref="IMemoryCache.Remove"/>
        public void Remove(object key)
        {
            _memoryCache.Remove(key);
        }

        /// <inheritdoc cref="IClearableInMemoryCache.Clear"/>
        public void Clear(string prefix = null)
        {
            if (prefix == null)
            {
                foreach (var cacheEntry in _cacheEntries.Keys.ToList())
                {
                    _memoryCache.Remove(cacheEntry);
                }
            }

            else
            {
                foreach (var cacheEntry in _cacheEntries.Keys.ToList())
                {
                    if (cacheEntry.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        _memoryCache.Remove(cacheEntry);
                    }
                }
            }

        }

        public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
        {
            return _cacheEntries.Select(pair => new KeyValuePair<object, object>(pair.Key, pair.Value.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets keys of all items in MemoryCache.
        /// </summary>
        public IEnumerator<object> Keys => _cacheEntries.Keys.GetEnumerator();
    }
}