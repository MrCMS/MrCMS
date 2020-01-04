using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MrCMS.DbConfiguration
{
    public class CheckInstallationStatus : ICheckInstallationStatus
    {
        private readonly ISystemDatabase _systemDatabase;
        private static bool _installed;
        public CheckInstallationStatus(ISystemDatabase systemDatabase)
        {
            _systemDatabase = systemDatabase;
        }
        public InstallationStatus GetStatus()
        {
            if (_installed)
                return InstallationStatus.Installed;
            try
            {
                var appliedMigrations = _systemDatabase.Database.GetAppliedMigrations();
                if (!appliedMigrations.Any())
                    return InstallationStatus.RequiresInstall;
                _installed = true;
                return InstallationStatus.Installed;
            }
            catch(Exception ex)
            {
                return InstallationStatus.NeedsConnectionString;
            }
        }

        public bool IsInstalled()
        {
            return _installed || GetStatus() == InstallationStatus.Installed;
        }
    }
}