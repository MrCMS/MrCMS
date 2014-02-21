using System.Collections.Generic;
using MrCMS.Settings;
using NHibernate.Util;

namespace MrCMS.HealthChecks
{
    public class IsEmailConfigured : HealthCheck
    {
        private readonly MailSettings _mailSettings;

        public IsEmailConfigured(MailSettings mailSettings)
        {
            _mailSettings = mailSettings;
        }

        public override string DisplayName
        {
            get { return "Email settings check"; }
        }

        public override HealthCheckResult PerformCheck()
        {
            var unsetFacets = new List<string>();
            if (string.IsNullOrWhiteSpace(_mailSettings.SystemEmailAddress))
                unsetFacets.Add("System email address is not set");
            if (string.IsNullOrWhiteSpace(_mailSettings.Host))
                unsetFacets.Add("Host is not set");
            if (string.IsNullOrWhiteSpace(_mailSettings.UserName))
                unsetFacets.Add("Username is not set");
            if (string.IsNullOrWhiteSpace(_mailSettings.Password))
                unsetFacets.Add("Password is not set");
            if (_mailSettings.Port <= 0)
                unsetFacets.Add("Port is not set");
            return !unsetFacets.Any()
                ? HealthCheckResult.Success
                : new HealthCheckResult { Messages = unsetFacets };
        }
    }
}