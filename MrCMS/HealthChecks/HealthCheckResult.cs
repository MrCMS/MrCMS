namespace MrCMS.HealthChecks
{
    public class HealthCheckResult
    {
        public HealthCheckResult(string check)
        {
            Check = check;
        }

        public bool OK { get; set; }
        public string Check { get; private set; }
        public string Message { get; set; }
    }
}