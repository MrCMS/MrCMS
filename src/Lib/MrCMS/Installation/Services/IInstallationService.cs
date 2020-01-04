using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Installation.Models;

namespace MrCMS.Installation.Services
{
    public interface IInstallationService
    {
        Task<InstallationResult> Install(InstallModel model);
        List<DatabaseProviderInfo> GetProviderTypes();
        bool DatabaseIsInstalled();
    }
}