using System.Collections.Generic;

namespace MrCMS.HealthChecks
{
    public class HealthCheckResult
    {
        public HealthCheckResult()
        {
            Messages = new List<string>();
            Status = HealthCheckStatus.Failure;
        }
        public HealthCheckStatus Status { get; set; }
        public List<string> Messages { get; set; }
        public static HealthCheckResult Success => new HealthCheckResult { Status = HealthCheckStatus.Success };
    }
}