using System;
using System.ComponentModel;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MrCMS.DbConfiguration;
using MrCMS.Settings;

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

        //public IPersistenceConfigurer GetPersistenceConfigurer()
        //{
        //    return
        //        SQLiteConfiguration.Standard.ConnectionString(builder => builder.Is(_databaseSettings.Value.ConnectionString));
        //}

        //public void AddProviderSpecificConfiguration(NHibernate.Cfg.Configuration config)
        //{
        //}

        public string Type
        {
            get { return GetType().FullName; }
        }

        public void SetupAction(IServiceProvider serviceProvider, DbContextOptionsBuilder builder, Assembly assembly)
        {
            builder.UseSqlite(_databaseSettings.Value.ConnectionString,
                optionsBuilder => optionsBuilder.MigrationsAssembly(assembly.FullName));
        }
    }

}