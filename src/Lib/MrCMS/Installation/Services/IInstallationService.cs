using System.Collections.Generic;
using MrCMS.Installation.Models;

namespace MrCMS.Installation.Services
{
    public interface IInstallationService
    {
        InstallationResult Install(InstallModel model);
        List<DatabaseProviderInfo> GetProviderTypes();
        bool DatabaseIsInstalled();
    }
}