using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Settings;

namespace MrCMS.HealthChecks
{
    public class LoginNotificationEmailsEnabled : HealthCheck
    {
        private readonly ISystemConfigurationProvider _configurationProvider;

        public LoginNotificationEmailsEnabled(ISystemConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public override string DisplayName => "Login Notification Emails Enabled";

        public override async Task<HealthCheckResult> PerformCheck()
        {
            SecuritySettings settings = await _configurationProvider.GetSystemSettings<SecuritySettings>();
            return settings.SendLoginNotificationEmails
                ? HealthCheckResult.Success
                : new HealthCheckResult
                {
                    Status = HealthCheckStatus.Warning,
                    Messages = new List<string>
                    {
                        "Login notification emails have not been enabled for this site."
                    }
                };
        }
    }
}