using MrCMS.HealthChecks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.Web.Apps.Admin.Services.Dashboard
{
    public interface IHealthCheckService
    {
        List<IHealthCheck> GetHealthChecks();
        Task<HealthCheckResult> CheckType(string typeName);
    }
}