using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.HealthChecks;

namespace MrCMS.Web.Areas.Admin.Services.Dashboard
{
    public interface IHealthCheckService
    {
        List<IHealthCheck> GetHealthChecks();
        Task<HealthCheckResult> CheckType(string typeName);
    }
}