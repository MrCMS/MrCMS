namespace MrCMS.HealthChecks
{
    public interface IHealthCheck
    {
        string DisplayName { get; }
        string TypeName { get; }
        HealthCheckResult PerformCheck();
    }
}