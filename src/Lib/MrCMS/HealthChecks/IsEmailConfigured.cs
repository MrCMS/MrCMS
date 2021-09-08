using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Settings;

namespace MrCMS.HealthChecks
{
    public class IsEmailConfigured : HealthCheck
    {
        private readonly MailSettings _mailSettings;

        public IsEmailConfigured(MailSettings mailSettings)
        {
            _mailSettings = mailSettings;
        }

        public override string DisplayName => "Email settings check";

        public override Task<HealthCheckResult> PerformCheck()
        {
            var unsetFacets = new List<string>();
            if (string.IsNullOrWhiteSpace(_mailSettings.SystemEmailAddress))
                unsetFacets.Add("System email address is not set");
            if (string.IsNullOrWhiteSpace(_mailSettings.Host))
                unsetFacets.Add("Host is not set");
            if (_mailSettings.Port <= 0)
                unsetFacets.Add("Port is not set");

            return Task.FromResult(!unsetFacets.Any()
                ? HealthCheckResult.Success
                : new HealthCheckResult { Messages = unsetFacets });
        }
    }
}