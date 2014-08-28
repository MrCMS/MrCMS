using System.ComponentModel;
using FluentNHibernate.Cfg.Db;
using MrCMS.Settings;

namespace MrCMS.DbConfiguration
{
    [Description("MySQL")]
    public class MySqlProvider : IDatabaseProvider
    {
        private readonly DatabaseSettings _databaseSettings;

        public MySqlProvider(DatabaseSettings databaseSettings)
        {
            _databaseSettings = databaseSettings;
        }

        public IPersistenceConfigurer GetPersistenceConfigurer()
        {
            return
                MySQLConfiguration.Standard.ConnectionString(builder => builder.Is(_databaseSettings.ConnectionString));
        }

        public string Type
        {
            get { return GetType().FullName; }
        }
    }
}