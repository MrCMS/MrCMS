using System;
using NHibernate;
using NHibernate.Caches.SysCache2;

namespace MrCMS.DbConfiguration.Caches.Azure
{
    public static class AzureCacheAdapterFactory
    {
        /// <summary>
        /// Factory method to create an app fabric cache adapter.
        /// </summary>
        /// <param name="regionName">The name of the AppFabric cache or region etc that the adapter will interface with.</param>
        /// <returns>An AppFabric cache adapter.</returns>
        public static AzureCacheAdapter Create(string regionName)
        {
            if (string.IsNullOrEmpty(regionName))
                throw new ArgumentNullException("A region name must be specified");
            return new AzureCacheAdapter(regionName);
        }
    }
}