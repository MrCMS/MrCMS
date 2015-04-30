using FluentNHibernate.Cfg.Db;
using MrCMS.DbConfiguration;
using NHibernate.Cfg;
using NHibernate.Dialect;

namespace MrCMS.Web.Tests
{
    public class SqliteInMemoryProvider : IDatabaseProvider
    {
        public string Type { get; private set; }

        public IPersistenceConfigurer GetPersistenceConfigurer()
        {
            return SQLiteConfiguration.Standard.Dialect<SQLiteDialect>()
                .InMemory()
                .Raw(Environment.ReleaseConnections, "on_close");
        }

        public void AddProviderSpecificConfiguration(Configuration config)
        {
        }
    }
}