using System.ComponentModel;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.Options;
using MrCMS.Settings;

namespace MrCMS.DbConfiguration
{
    [Description("Use SQL Server 2012 (or SQL Express) database.")]
    public class SqlServer2012Provider : IDatabaseProvider
    {
        private readonly IOptions<DatabaseSettings> _databaseSettings;

        public SqlServer2012Provider(IOptions<DatabaseSettings> databaseSettings)
        {
            _databaseSettings = databaseSettings;
        }

        public IPersistenceConfigurer GetPersistenceConfigurer()
        {
            return MsSqlConfiguration.MsSql2012.ConnectionString(x => x.Is(_databaseSettings.Value.ConnectionString));
        }

        public void AddProviderSpecificConfiguration(NHibernate.Cfg.Configuration config)
        {
            SqlServerGuidHelper.SetGuidToUniqueWithDefaultValue(config);
        }

        public string Type => GetType().FullName;
    }
}