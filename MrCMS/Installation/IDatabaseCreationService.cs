using System.Collections.Generic;
using MrCMS.DbConfiguration;

namespace MrCMS.Installation
{
    public interface IDatabaseCreationService
    {
        IDatabaseProvider CreateDatabase(InstallModel model);
    }
}