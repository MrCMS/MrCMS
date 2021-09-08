using System.ComponentModel;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.Options;
using MrCMS.DbConfiguration;
using MrCMS.Settings;
using NHibernate.Cfg.Loquacious;

namespace MrCMS.Data.Sqlite
{
    [Description("Use built-in data storage (Sqlite) (limited compatibility).")]
    [NoConnectionStringBuilder]
    public class SqliteProvider : IDatabaseProvider
    {
        private readonly IOptions<DatabaseSettings> _databaseSettings;

        public SqliteProvider(IOptions<DatabaseSettings> databaseSettings)
        {
            _databaseSettings = databaseSettings;
        }

        public IPersistenceConfigurer GetPersistenceConfigurer()
        {
            return
                SQLiteConfiguration.Standard.ConnectionString(builder =>
                    builder.Is(_databaseSettings.Value.ConnectionString));
        }

        public void AddProviderSpecificConfiguration(NHibernate.Cfg.Configuration config)
        {
        }

        public void DebugDatabaseIntegration(DbIntegrationConfigurationProperties properties)
        {
            var logQueries = _databaseSettings.Value.LogQueries;
            properties.LogFormattedSql = logQueries;
            properties.LogSqlInConsole = logQueries;
        }

        public string Type
        {
            get { return GetType().FullName; }
        }
    }
}