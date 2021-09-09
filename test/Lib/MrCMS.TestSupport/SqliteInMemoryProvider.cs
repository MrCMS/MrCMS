using FluentNHibernate.Cfg.Db;
using MrCMS.DbConfiguration;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Dialect;

namespace MrCMS.TestSupport
{
    public class SqliteInMemoryProvider : IDatabaseProvider
    {
        public string Type { get; private set; }

        public IPersistenceConfigurer GetPersistenceConfigurer()
        {
            return SQLiteConfiguration.Standard.Dialect<SQLiteDialect>()
                .InMemory()
                .Raw(NHibernate.Cfg.Environment.ReleaseConnections, "on_close");
        }

        public void AddProviderSpecificConfiguration(Configuration config)
        {
        }

        public void DebugDatabaseIntegration(DbIntegrationConfigurationProperties properties)
        {
            properties.LogFormattedSql = false;
            properties.LogSqlInConsole = false;
        }
    }
}