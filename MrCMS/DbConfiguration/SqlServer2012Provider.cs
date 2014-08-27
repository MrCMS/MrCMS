using System.ComponentModel;
using FluentNHibernate.Cfg.Db;
using MrCMS.Settings;

namespace MrCMS.DbConfiguration
{
    [Description("Use SQL Server 2012 (or SQL Express) database.")]
    public class SqlServer2012Provider : IDatabaseProvider
    {
        private readonly DatabaseSettings _databaseSettings;

        public SqlServer2012Provider(DatabaseSettings databaseSettings)
        {
            _databaseSettings = databaseSettings;
        }

        public IPersistenceConfigurer GetPersistenceConfigurer()
        {
            return MsSqlConfiguration.MsSql2012.ConnectionString(x => x.Is(_databaseSettings.ConnectionString));
        }

        public string Type
        {
            get { return GetType().FullName; }
        }
    }
}