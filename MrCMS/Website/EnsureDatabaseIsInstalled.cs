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
            // if connection strings are stored in new location, all is well
            var databaseSettings = _systemConfigurationProvider.GetSystemSettings<DatabaseSettings>();
            if (!string.IsNullOrWhiteSpace(databaseSettings.ConnectionString))
                return true;
            ConnectionStringSettings connectionString;
            try
            {
                // otherwise check the connection string
                connectionString = ConfigurationManager.ConnectionStrings["mrcms"];
                if (connectionString == null)
                    return false;
            }
            catch
            {
                return false;
            }

            databaseSettings.ConnectionString = connectionString.ConnectionString;
            databaseSettings.DatabaseProviderType = GetProviderType(connectionString.ProviderName);
            _systemConfigurationProvider.SaveSettings(databaseSettings);
            return true;
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