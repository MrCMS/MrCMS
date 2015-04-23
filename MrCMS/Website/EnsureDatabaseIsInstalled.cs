using System;
using System.Configuration;
using MrCMS.DbConfiguration;
using MrCMS.Helpers;
using MrCMS.Settings;

namespace MrCMS.Website
{
    public class EnsureDatabaseIsInstalled : IEnsureDatabaseIsInstalled
    {
        private readonly ISystemConfigurationProvider _systemConfigurationProvider;

        public EnsureDatabaseIsInstalled(ISystemConfigurationProvider systemConfigurationProvider)
        {
            _systemConfigurationProvider = systemConfigurationProvider;
        }

        public bool IsInstalled()
        {
            DatabaseSettings databaseSettings = new DatabaseSettings();
            if (_systemConfigurationProvider.Exists<DatabaseSettings>())
            // if connection strings are stored in new location, all is well
            {
                databaseSettings = _systemConfigurationProvider.GetSystemSettings<DatabaseSettings>();
                if (!string.IsNullOrWhiteSpace(databaseSettings.ConnectionString))
                    return true;
            }
            ConnectionStringSettings connectionString = GetConnectionStringSetting();
            if (connectionString == null)
                return false;

            databaseSettings.ConnectionString = connectionString.ConnectionString;
            databaseSettings.DatabaseProviderType = GetProviderType(connectionString.ProviderName);
            _systemConfigurationProvider.SaveSettings(databaseSettings);
            return true;
        }

        private static ConnectionStringSettings GetConnectionStringSetting()
        {
            ConnectionStringSettings connectionString = null;
            try
            {
                // otherwise check the connection string
                connectionString = ConfigurationManager.ConnectionStrings["mrcms"];
            }
            catch
            {
            }
            return connectionString;
        }

        private string GetProviderType(string providerName)
        {
            if (providerName.Contains("sqlite", StringComparison.OrdinalIgnoreCase))
                return typeof(SqliteProvider).FullName;
            if (providerName.Contains("mysql", StringComparison.OrdinalIgnoreCase))
                return typeof(MySqlProvider).FullName;
            return typeof(SqlServer2008Provider).FullName;
        }
    }
}