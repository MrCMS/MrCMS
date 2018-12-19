using MrCMS.HealthChecks;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Admin.Services.Dashboard
{
    public interface IHealthCheckService
    {
        List<IHealthCheck> GetHealthChecks();
        HealthCheckResult CheckType(string typeName);
    }
}