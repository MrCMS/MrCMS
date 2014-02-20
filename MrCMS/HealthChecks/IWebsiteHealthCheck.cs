namespace MrCMS.HealthChecks
{
    public interface IWebsiteHealthCheck
    {
        string DisplayName { get; }
        string TypeName { get; }
        HealthCheckResult PerformCheck();
    }
}