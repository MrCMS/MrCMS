using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MrCMS.Settings;

namespace MrCMS.DbConfiguration
{
    public class CheckInstallationStatus : ICheckInstallationStatus
    {
        private readonly IOptionsMonitor<DatabaseSettings> _monitor;
        private readonly ISystemDatabase _systemDatabase;
        private static bool _installed;
        public CheckInstallationStatus(IOptionsMonitor<DatabaseSettings> monitor, ISystemDatabase systemDatabase)
        {
            _monitor = monitor;
            _systemDatabase = systemDatabase;
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

                var migrations = _systemDatabase.Database.GetMigrations();
                if (!migrations.Any())
                    return InstallationStatus.RequiresMigrations;
                if(!_systemDatabase.Database.GetAppliedMigrations().Any() || !_systemDatabase.IsMrCMSInstalled)
                    return InstallationStatus.RequiresInstallation;
                _installed = true;
                return InstallationStatus.Installed;
            }
            catch
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