using System.Collections.Generic;
using System.Linq;
using MrCMS.ACL;
using MrCMS.Tasks;

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

        public override HealthCheckResult PerformCheck()
        {

            return _settings.ACLEnabled
                ? HealthCheckResult.Success
                : new HealthCheckResult
                {
                    Status = HealthCheckStatus.Warning,
                    Messages = new List<string>
                    {
                        "ACL has not been enabled for this site."
                    }
                };
        }
    }
}