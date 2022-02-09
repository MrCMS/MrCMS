using System.Threading.Tasks;

namespace MrCMS.HealthChecks
{
    public interface IHealthCheck
    {
        string DisplayName { get; }
        string TypeName { get; }
        Task<HealthCheckResult> PerformCheck();
    }
}