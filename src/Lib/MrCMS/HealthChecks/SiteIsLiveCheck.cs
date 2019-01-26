using System.Collections.Generic;
using MrCMS.Settings;

namespace MrCMS.HealthChecks
{
    public class SiteIsLive : HealthCheck
    {
        private readonly SiteSettings _siteSettings;

        public SiteIsLive(SiteSettings siteSettings)
        {
            _siteSettings = siteSettings;
        }

        public override string DisplayName
        {
            get { return "Site in live mode"; }
        }

        public override HealthCheckResult PerformCheck()
        {
           
            if (!_siteSettings.SiteIsLive)
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