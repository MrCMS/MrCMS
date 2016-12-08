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
            var databaseSettings = _systemConfigurationProvider.GetSystemSettings<DatabaseSettings>();
            return !string.IsNullOrWhiteSpace(databaseSettings.ConnectionString);
        }
    }
}