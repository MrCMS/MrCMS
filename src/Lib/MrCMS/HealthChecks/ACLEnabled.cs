using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.ACL;

namespace MrCMS.HealthChecks
{
    public class ACLEnabled : HealthCheck
    {
        private readonly ACLSettings _settings;

        public ACLEnabled(ACLSettings settings)
        {
            _settings = settings;
        }

        public override string DisplayName => "ACL Enabled";

        public override Task<HealthCheckResult> PerformCheck()
        {

            return Task.FromResult(_settings.ACLEnabled
                ? HealthCheckResult.Success
                : new HealthCheckResult
                {
                    Status = HealthCheckStatus.Warning,
                    Messages = new List<string>
                    {
                        "ACL has not been enabled for this site."
                    }
                });
        }
    }
}