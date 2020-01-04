using System;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MrCMS.Settings;

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

        public string Type
        {
            get { return GetType().FullName; }
        }

        public void SetupAction(IServiceProvider serviceProvider, DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer(_databaseSettings.Value.ConnectionString);
        }
    }
}