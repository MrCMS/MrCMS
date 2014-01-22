using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using Microsoft.ApplicationServer.Caching;
using MrCMS.Config;
using MrCMS.Website;
using NHibernate;
using NHibernate.Cache;

namespace MrCMS.DbConfiguration.Caches.Azure
{
    /// <summary>
    /// Pluggable cache implementation using the System.Web.Caching classes
    /// </summary>
    public class AzureCacheAdapter : ICache
    {
        /// <summary>The name of the cache prefix to differntaite the nhibernate cache elements from
        /// other items in the cache</summary>
        private const string CacheKeyPrefix = "NHibernate-Cache:";

        /// <summary>logger for the type</summary>
        private static readonly IInternalLogger Log = LoggerProvider.LoggerFor((typeof(AzureCacheAdapter)));

        /// <summary>the name of the cache region</summary>
        private readonly string _name;

        /// <summary>The cache for the web application</summary>
        private readonly DataCache _webCache;

        private readonly ISerializationProvider _serializationProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureCacheAdapter"/> class with
        /// the default configuration properties
        /// </summary>
        public AzureCacheAdapter() : this(null) { }


        /// <summary>
        /// Initializes a new instance of the <see cref="AzureCacheAdapter"/> class.
        /// </summary>
        /// <param name="name">The name of the region</param>
        /// <param name="serializationProvider"></param>
        public AzureCacheAdapter(string name)
        {
            _serializationProvider = MrCMSApplication.Get<ISerializationProvider>();
            //validate the params
            if (String.IsNullOrEmpty(name))
            {
                Log.Info("No region name specified for cache region. Using default name of 'nhibernate'");
                name = "nhibernate";
            }
            var mrCMSSection = WebConfigurationManager.GetSection("mrcms") as MrCMSConfigSection;
            _webCache = AzureCacheFactory.Instance.GetCache(mrCMSSection == null ? "default" : mrCMSSection.CacheName);
            _name = StripNonAlphaNumeric(name);

            //configure the cache region based on the configured settings and any relevant nhibernate settings

        }

        private string StripNonAlphaNumeric(string name)
        {
            return name.Where(c => char.IsLetter(c) || char.IsDigit(c)).Aggregate("", (current, c) => current + c);
        }

        /// <summary>
        /// Clear the Cache
        /// </summary>
        /// <exception cref="T:NHibernate.Cache.CacheException"></exception>
        public void Clear()
        {
            //remove the root cache item, this will cause all of the individual items to be removed from the cache
            _webCache.ClearRegion(RegionName);

            Log.Debug("All items cleared from the cache.");
        }

        /// <summary>
        /// Clean up.
        /// </summary>
        /// <exception cref="T:NHibernate.Cache.CacheException"></exception>
        public void Destroy()
        {
            Clear();
        }

        /// <summary>
        /// Get the object from the Cache
        /// </summary>
        /// <param name="key">the id of the item to get from the cache</param>
        /// <returns>the item stored in the cache with the id specified by <paramref name="key"/></returns>
        public object Get(object key)
        {
            if (key == null)
                return null;

            //get the full key to use to locate the item in the cache
            string cacheKey = GetCacheKey(key);

            if (Log.IsDebugEnabled)
                Log.DebugFormat("Fetching object '{0}' from the cache.", cacheKey);

            object cachedObject = _webCache.Get(cacheKey, RegionName);

            if (cachedObject == null || !(cachedObject is byte[]))
            {
                if (_name == "UpdateTimestampsCache")
                {
                    Put(key, long.MinValue);
                    return long.MinValue;
                }
                return null;
            }

            return _serializationProvider.Deserialize(cachedObject as byte[]);
        }

        /// <summary>
        /// If this is a clustered cache, lock the item
        /// </summary>
        /// <param name="key">The Key of the Item in the Cache to lock.</param>
        /// <exception cref="T:NHibernate.Cache.CacheException"></exception>
        public void Lock(object key)
        {
            //nothing to do here
        }

        /// <summary>
        /// Generate a timestamp
        /// </summary>
        /// <returns>a timestamp</returns>
        public long NextTimestamp()
        {
            return Timestamper.Next();
        }

        /// <summary>Puts an item into the cache
        /// </summary>
        /// <param name="key">the key of the item to cache</param>
        /// <param name="value">the actual value/object to cache</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> or
        /// <paramref name="value"/> is null.</exception>
        [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope")]
        public void Put(object key, object value)
        {
            //validate the params
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            //get the full key for the cache key
            string cacheKey = GetCacheKey(key);

            try
            {
                _webCache.Put(cacheKey, _serializationProvider.Serialize(value), RegionName);
            }
            catch (DataCacheException ex)
            {
                if (ex.ErrorCode == DataCacheErrorCode.RegionDoesNotExist)
                {
                    CreateAppFabricRegion(b => Put(key, value));
                }
                else if (!IsSafeToIgnore(ex) && ex.ErrorCode != DataCacheErrorCode.ObjectLocked)
                {
                    throw new CacheException(ex);
                }
            }
        }

        /// <summary>
        /// Checks whether the execption thrown by the AppFabric client is safe to ignore or not. Sometime sommunication between
        /// the client and server can be slow for example in which case the client will throw na exception, but we would want to
        /// ignore it.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private bool IsSafeToIgnore(DataCacheException ex)
        {
            return ex.ErrorCode == DataCacheErrorCode.ConnectionTerminated ||
                   ex.ErrorCode == DataCacheErrorCode.RetryLater || ex.ErrorCode == DataCacheErrorCode.Timeout;
        }

        /// <summary>
        /// Creates the App Fabric cache region used for caching items.
        /// </summary>
        /// <param name="callback">A call back to call once the region has been created.</param>
        private void CreateAppFabricRegion(Action<bool> callback)
        {
            CreateRegion(_webCache, RegionName, callback);
        }

        /// <summary>
        /// Creates an AppFabric cache region.
        /// </summary>
        /// <param name="cache">The data cache [client] to use to create the cache region.</param>
        /// <param name="regionName">The name of the AppFabric cache region to create.</param>
        /// <param name="callback">A call back to call once the region has been created.</param>
        private void CreateRegion(DataCache cache, string regionName, Action<bool> callback)
        {
            try
            {
                callback(cache.CreateRegion(regionName));
            }
            catch (DataCacheException ex)
            {
                if (ex.ErrorCode == DataCacheErrorCode.RegionAlreadyExists)
                    callback(false);
                else
                {
                    throw new CacheException(ex);
                }
            }
        }

        /// <summary>
        /// Gets the name of the cache region
        /// </summary>
        public string RegionName
        {
            get { return _name; }
        }

        /// <summary>
        /// Remove an item from the Cache.
        /// </summary>
        /// <param name="key">The Key of the Item in the Cache to remove.</param>
        /// <exception cref="T:NHibernate.Cache.CacheException"></exception>
        /// <exception cref="ArgumentNullException">thrown if <paramref name="key"/> is null.</exception>
        public void Remove(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            //get the full cache key
            string cacheKey = GetCacheKey(key);

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("removing item with key:", cacheKey);
            }

            _webCache.Remove(cacheKey);
        }

        /// <summary>
        /// Get a reasonable "lock timeout"
        /// </summary>
        /// <value></value>
        public int Timeout
        {
            get
            {
                return Timestamper.OneMs * 60000; // 60 seconds
            }
        }

        /// <summary>
        /// If this is a clustered cache, unlock the item
        /// </summary>
        /// <param name="key">The Key of the Item in the Cache to unlock.</param>
        /// <exception cref="T:NHibernate.Cache.CacheException"></exception>
        public void Unlock(object key)
        {
            //nothing to do since we arent locking
        }

        /// <summary>
        /// Gets a valid cache key for the element in the cache with <paramref name="identifier"/>.
        /// </summary>
        /// <param name="identifier">The identifier of a cache element</param>
        /// <returns>Key to use for retrieving an element from the cache</returns>
        private string GetCacheKey(object identifier)
        {
            return String.Concat(CacheKeyPrefix, _name, ":", identifier.ToString(), "@", identifier.GetHashCode());
        }
    }
}