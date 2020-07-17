using MrCMS.HealthChecks;
using System.Collections.Generic;

namespace MrCMS.Web.Admin.Services.Dashboard
{
    public interface IHealthCheckService
    {
        List<IHealthCheck> GetHealthChecks();
        HealthCheckResult CheckType(string typeName);
    }
}