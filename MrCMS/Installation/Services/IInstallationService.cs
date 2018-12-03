using System.Collections.Generic;

namespace MrCMS.Installation.Services
{
    public interface IInstallationService
    {
        InstallationResult Install(InstallModel model);
        List<DatabaseProviderInfo> GetProviderTypes();
    }
}