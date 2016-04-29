using System;
using System.Web.Configuration;
using FluentNHibernate.Cfg.Db;
using NHibernate.Caches.Redis;
using NHibernate.Caches.SysCache2;
using StackExchange.Redis;

namespace MrCMS.DbConfiguration.Caches.Redis
{
    public class RedisCacheInitializer : CacheProviderInitializer<RequestRecoveryRedisCacheProvider>
    {
        private static readonly Lazy<ConnectionMultiplexer> lazyConnection =
            new Lazy<ConnectionMultiplexer>(
                () =>
                    ConnectionMultiplexer.Connect(ConnectionString));

        private static string ConnectionString
        {
            get { return WebConfigurationManager.AppSettings["redis-cache-connection-string"]; }
        }

        public static ConnectionMultiplexer Multiplexer
        {
            get { return lazyConnection.Value; }
        }

        public static bool Initialized { get; private set; }

        public override void Initialize(CacheSettingsBuilder builder)
        {
            try
            {
                TrySetMultiplexer();
                builder.ProviderClass<RequestRecoveryRedisCacheProvider>();
            }
            catch
            {
                builder.ProviderClass<SysCacheProvider>();
            }
        }

        private static void TrySetMultiplexer()
        {
            if (Initialized)
                return;
            RedisCacheProvider.SetConnectionMultiplexer(Multiplexer);
            Multiplexer.GetDatabase();
            Initialized = true;
        }

        public static void Dispose()
        {
            Multiplexer.Dispose();
        }
    }
}