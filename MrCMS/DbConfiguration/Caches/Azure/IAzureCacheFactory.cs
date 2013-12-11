using Microsoft.ApplicationServer.Caching;

namespace MrCMS.DbConfiguration.Caches.Azure
{
    /// <summary>
    /// Defines an interface for creating/getting DataCache [clients].
    /// </summary>
    public interface IAzureCacheFactory
    {
        /// <summary>
        /// Gets an instance of a data cache [client].
        /// </summary>
        /// <param name="cacheName">The name of the AppFabric cache to get the data cache [client] for.</param>
        /// <param name="useDefault">A flag to determine whether or not the default cache should be used if the named cache
        /// does not exist.</param>
        /// <returns>A data cache [client].</returns>
        DataCache GetCache(string cacheName, bool useDefault = false);
    }
}