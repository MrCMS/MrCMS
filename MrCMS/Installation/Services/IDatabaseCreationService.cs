using MrCMS.DbConfiguration;

namespace MrCMS.Installation.Services
{
    public interface IDatabaseCreationService
    {
        InstallationResult ValidateConnectionString(InstallModel model);
        IDatabaseProvider CreateDatabase(InstallModel model);
    }
}