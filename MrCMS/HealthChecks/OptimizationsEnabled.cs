using System.Collections.Generic;
using MrCMS.Services.Resources;
using MrCMS.Settings;

namespace MrCMS.HealthChecks
{
    public class OptimizationsEnabled : HealthCheck
    {
        private readonly BundlingSettings _bundlingSettings;
        private readonly IStringResourceProvider _stringResourceProvider;

        public OptimizationsEnabled(BundlingSettings bundlingSettings, IStringResourceProvider stringResourceProvider)
        {
            _bundlingSettings = bundlingSettings;
            _stringResourceProvider = stringResourceProvider;
        }

        public override HealthCheckResult PerformCheck()
        {
            if (_bundlingSettings.EnableOptimisations)
                return new HealthCheckResult
                {
                    OK = true
                };

            return new HealthCheckResult
            {
                OK = false,
                Messages = new List<string>
                {
                    _stringResourceProvider.GetValue("Please enable optimizations in system settings.")
                }
            };
        }
    }
}