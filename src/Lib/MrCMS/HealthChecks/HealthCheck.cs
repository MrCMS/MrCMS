using System.Threading.Tasks;
using MrCMS.Helpers;

namespace MrCMS.HealthChecks
{
    public abstract class HealthCheck : IHealthCheck
    {
        public virtual string DisplayName => GetType().Name.BreakUpString();
        public string TypeName => GetType().FullName;
        public abstract Task<HealthCheckResult> PerformCheck();
    }
}