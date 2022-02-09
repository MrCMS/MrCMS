using System.ComponentModel;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.Options;
using MrCMS.Settings;
using NHibernate.Cfg.Loquacious;

namespace MrCMS.DbConfiguration
{
    [Description("Use SQL Server 2008 (or SQL Express) database.")]
    public class SqlServer2008Provider : IDatabaseProvider
    {
        private readonly IOptions<DatabaseSettings> _databaseSettings;

        public SqlServer2008Provider(IOptions<DatabaseSettings> databaseSettings)
        {
            _databaseSettings = databaseSettings;
        }

        public IPersistenceConfigurer GetPersistenceConfigurer()
        {
            return MsSqlConfiguration.MsSql2008.ConnectionString(x => x.Is(_databaseSettings.Value.ConnectionString));
        }

        public void AddProviderSpecificConfiguration(NHibernate.Cfg.Configuration config)
        {
            SqlServerGuidHelper.SetGuidToUniqueWithDefaultValue(config);
        }

        public void DebugDatabaseIntegration(DbIntegrationConfigurationProperties properties)
        {
            var logQueries = _databaseSettings.Value.LogQueries;
            properties.LogFormattedSql = logQueries;
            properties.LogSqlInConsole = logQueries;
        }

        public string Type => GetType().FullName;
    }
}