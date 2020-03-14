using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Settings;

namespace MrCMS.HealthChecks
{
    public class IsEmailConfigured : HealthCheck
    {
        private readonly ISystemConfigurationProvider _configurationProvider;

        public IsEmailConfigured(ISystemConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public override string DisplayName
        {
            get { return "Email settings check"; }
        }

        public override async Task<HealthCheckResult> PerformCheck()
        {
            var mailSettings = await _configurationProvider.GetSystemSettings<MailSettings>();
            var unsetFacets = new List<string>();
            if (string.IsNullOrWhiteSpace(mailSettings.SystemEmailAddress))
                unsetFacets.Add("System email address is not set");
            if (string.IsNullOrWhiteSpace(mailSettings.Host))
                unsetFacets.Add("Host is not set");
            if (mailSettings.Port <= 0)
                unsetFacets.Add("Port is not set");

            return !unsetFacets.Any()
                ? HealthCheckResult.Success
                : new HealthCheckResult {Messages = unsetFacets};
        }
    }
}