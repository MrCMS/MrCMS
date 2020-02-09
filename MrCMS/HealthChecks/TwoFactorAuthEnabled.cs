using System.Collections.Generic;
using MrCMS.Settings;

namespace MrCMS.HealthChecks
{
    public class TwoFactorAuthEnabled : HealthCheck
    {
        private readonly SecuritySettings _settings;

        public TwoFactorAuthEnabled(SecuritySettings settings)
        {
            _settings = settings;
        }

        public override string DisplayName => "2FA Enabled";

        public override HealthCheckResult PerformCheck()
        {

            return _settings.TwoFactorAuthEnabled
                ? HealthCheckResult.Success
                : new HealthCheckResult
                {
                    Status = HealthCheckStatus.Warning,
                    Messages = new List<string>
                    {
                        "2FA has not been enabled for this site."
                    }
                };
        }
    }
}