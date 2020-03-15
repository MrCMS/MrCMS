using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MrCMS.Settings;

namespace MrCMS.DbConfiguration
{
    public class CheckInstallationStatus : ICheckInstallationStatus
    {
        private readonly IOptionsMonitor<DatabaseSettings> _monitor;
        private readonly IServiceProvider _serviceProvider;
        private static bool _installed;
        public CheckInstallationStatus(IOptionsMonitor<DatabaseSettings> monitor, IServiceProvider serviceProvider)
        {
            _monitor = monitor;
            _serviceProvider = serviceProvider;
        }
        public InstallationStatus GetStatus()
        {
            if (_installed)
                return InstallationStatus.Installed;
            try
            {
                var settings = _monitor.CurrentValue;
                if (!settings.AreSet())
                    return InstallationStatus.RequiresDatabaseSettings;

                using (var scope = _serviceProvider.CreateScope())
                {
                    var systemDatabase = scope.ServiceProvider.GetRequiredService<ISystemDatabase>();
                    var migrations = systemDatabase.Database.GetMigrations();
                    if (!migrations.Any())
                        return InstallationStatus.RequiresMigrations;
                    if (!systemDatabase.Database.GetAppliedMigrations().Any() || !systemDatabase.IsMrCMSInstalled)
                        return InstallationStatus.RequiresInstallation;
                    _installed = true;
                }

                return InstallationStatus.Installed;
            }
#pragma warning disable 168
            catch (Exception exception)
#pragma warning restore 168
            {
                return InstallationStatus.RequiresDatabaseSettings;
            }
        }

        public bool IsInstalled()
        {
            return _installed || GetStatus() == InstallationStatus.Installed;
        }
    }
}