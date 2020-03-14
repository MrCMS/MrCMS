using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.ACL;
using MrCMS.Settings;
using MrCMS.Tasks;

namespace MrCMS.HealthChecks
{
    public class ACLEnabled : HealthCheck
    {
        private readonly IConfigurationProvider _configurationProvider;

        public ACLEnabled(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public override string DisplayName => "ACL Enabled";

        public override async Task<HealthCheckResult> PerformCheck()
        {
            var settings = await _configurationProvider.GetSiteSettings<ACLSettings>();

            return settings.ACLEnabled
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