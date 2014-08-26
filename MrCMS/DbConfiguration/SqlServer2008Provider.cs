using System.ComponentModel;
using FluentNHibernate.Cfg.Db;
using MrCMS.Settings;

namespace MrCMS.DbConfiguration
{
    [Description("Use SQL Server 2008 (or SQL Express) database.")]
    public class SqlServer2008Provider : IDatabaseProvider
    {
        private readonly DatabaseSettings _databaseSettings;

        public SqlServer2008Provider(DatabaseSettings databaseSettings)
        {
            _databaseSettings = databaseSettings;
        }

        public IPersistenceConfigurer GetPersistenceConfigurer()
        {
            return MsSqlConfiguration.MsSql2008.ConnectionString(x => x.Is(_databaseSettings.ConnectionString));
        }

        public string Type
        {
            get { return GetType().FullName; }
        }
    }
}