using System.ComponentModel;
using System.Data.Common;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.Options;
using MrCMS.Settings;
using NHibernate.Cfg.Loquacious;
using NHibernate.Driver;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;

namespace MrCMS.DbConfiguration;

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
        return MsSqlConfiguration.MsSql2012
            .ConnectionString(x => x.Is(_databaseSettings.Value.ConnectionString))
            .Driver<MiniProfiledSqlClientDriver>();
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

public class MiniProfiledSqlClientDriver : MicrosoftDataSqlClientDriver
{
    public override DbCommand CreateCommand()
    {
        var dbCommand = base.CreateCommand();
        if (MiniProfiler.Current != null)
        {
            dbCommand = new ProfiledDbCommand(
                dbCommand, null, MiniProfiler.Current
            );
        }

        return dbCommand;
    }
}