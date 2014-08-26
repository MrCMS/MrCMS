using System.ComponentModel;
using FluentNHibernate.Cfg.Db;
using MrCMS.Settings;

namespace MrCMS.DbConfiguration
{
    [Description("Use built-in data storage (Sqlite) (limited compatibility).")]
    [NoConnectionStringBuilder]
    public class SqliteProvider : IDatabaseProvider
    {
        private readonly DatabaseSettings _databaseSettings;

        public SqliteProvider(DatabaseSettings databaseSettings)
        {
            _databaseSettings = databaseSettings;
        }

        public IPersistenceConfigurer GetPersistenceConfigurer()
        {
            return
                SQLiteConfiguration.Standard.ConnectionString(builder => builder.Is(_databaseSettings.ConnectionString));
        }

        public string Type
        {
            get { return GetType().FullName; }
        }
    }
}