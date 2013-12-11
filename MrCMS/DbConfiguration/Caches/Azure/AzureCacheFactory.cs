using System;
using System.Collections.Generic;
using System.Security;
using Microsoft.ApplicationServer.Caching;

namespace MrCMS.DbConfiguration.Caches.Azure
{
    /// <summary>
    /// A singleton implementation of the <see cref="IAppFabricCacheFactory"/> for creating data cache [clients].
    /// </summary>
    public class AzureCacheFactory : IAzureCacheFactory
    {
        private static readonly AzureCacheFactory _instance = new AzureCacheFactory();

        private DataCacheFactory _cacheCluster;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static AzureCacheFactory()
        {
        }

        /// <summary>
        /// A lazy thread-safe singleton implemnatation due to the cost of creating <see cref="DataCacheFactory"/>.
        /// </summary>
        private AzureCacheFactory()
        {
            _cacheCluster = new DataCacheFactory();
        }

        /// <summary>
        /// The current singleton instance of the AppFabricCacheFactory.
        /// </summary>
        public static AzureCacheFactory Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Gets an instance of a data cache [client].
        /// </summary>
        /// <param name="cacheName">The name of the AppFabric cache to get the data cache [client] for.</param>
        /// <param name="useDefault">A flag to determine whether or not the default cache should be used if the named cache
        /// does not exist.</param>
        /// <returns>A data cache [client].</returns>
        public DataCache GetCache(string cacheName, bool useDefault = false)
        {
            try
            {
                return _cacheCluster.GetCache(cacheName);
            }
            catch (DataCacheException ex)
            {
                if (ex.ErrorCode == DataCacheErrorCode.NamedCacheDoesNotExist && useDefault)
                    return _cacheCluster.GetDefaultCache();

                throw;
            }
        }
    }
}