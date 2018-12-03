using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MrCMS.Helpers;
using MrCMS.Settings;
using System;

namespace MrCMS.DbConfiguration
{
    public class DatabaseProviderResolver : IDatabaseProviderResolver
    {
        private readonly IOptions<DatabaseSettings> _databaseSettings;
        private readonly IServiceProvider _serviceProvider;

        public DatabaseProviderResolver(IOptions<DatabaseSettings> databaseSettings, IServiceProvider serviceProvider)
        {
            _databaseSettings = databaseSettings;
            _serviceProvider = serviceProvider;
        }
        public IDatabaseProvider GetProvider()
        {
            var settings = _databaseSettings.Value;
            var typeByName = TypeHelper.GetTypeByName(settings.DatabaseProviderType);
            if (typeByName == null)
            {
                return null;
            }

            return _serviceProvider.GetRequiredService(typeByName) as IDatabaseProvider;
        }

        public bool IsProviderConfigured()
        {
            return GetProvider() != null;
        }
    }
}