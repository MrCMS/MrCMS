using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Settings;

namespace MrCMS.HealthChecks
{
    public class SiteIsLive : HealthCheck
    {
        private readonly IConfigurationProvider _configurationProvider;

        public SiteIsLive(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public override string DisplayName
        {
            get { return "Site in live mode"; }
        }

        public override async Task<HealthCheckResult> PerformCheck()
        {
            var siteSettings = await _configurationProvider.GetSiteSettings<SiteSettings>();
            if (!siteSettings.SiteIsLive)
            {
                return new HealthCheckResult
                {
                    Messages = new List<string> { "The current site is not set to live, please change this in site settings." },
                    Status = HealthCheckStatus.Failure
                };
            }

            return HealthCheckResult.Success;
        }
    }
}