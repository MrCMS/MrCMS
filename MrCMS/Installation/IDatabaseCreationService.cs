using System.Collections.Generic;
using MrCMS.DbConfiguration;

namespace MrCMS.Installation
{
    public interface IDatabaseCreationService
    {
        InstallationResult ValidateConnectionString(InstallModel model);
        IDatabaseProvider CreateDatabase(InstallModel model);
    }
}