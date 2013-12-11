using System.Collections.Generic;
using System.Configuration;
using NHibernate;
using NHibernate.Cache;

namespace MrCMS.DbConfiguration.Caches.Azure
{
    public class AzureCacheProvider : ICacheProvider
    {
        /// <summary>
        /// Configure the cache
        /// </summary>
        /// <param name="regionName">the name of the cache region</param>
        /// <param name="properties">configuration settings</param>
        /// <returns></returns>
        public ICache BuildCache(string regionName, IDictionary<string, string> properties)
        {
            return AzureCacheAdapterFactory.Create(regionName);
        }

        /// <summary>
        /// generate a timestamp
        /// </summary>
        /// <returns></returns>
        public long NextTimestamp()
        {
            return Timestamper.Next();
        }

        /// <summary>
        /// Callback to perform any necessary initialization of the underlying cache implementation
        /// during ISessionFactory construction.
        /// </summary>
        /// <param name="properties">current configuration settings</param>
        public void Start(IDictionary<string, string> properties)
        {
        }

        /// <summary>
        /// Callback to perform any necessary cleanup of the underlying cache implementation
        /// during <see cref="ISessionFactory.Close" />.
        /// </summary>
        public void Stop()
        {
        }
    }
}