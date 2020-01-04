using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Tasks;

namespace MrCMS.HealthChecks
{
    public class PublishWebpageTaskHealthCheck : HealthCheck
    {
        private readonly ITaskSettingManager _taskSettingManager;

        public PublishWebpageTaskHealthCheck(ITaskSettingManager taskSettingManager)
        {
            _taskSettingManager = taskSettingManager;
        }

        public override string DisplayName
        {
            get { return "Page Publisher task setup"; }
        }

        public override async Task<HealthCheckResult> PerformCheck()
        {
            var info = await _taskSettingManager.GetInfo();
            var any = info.Any(x => x.Type == typeof(PublishScheduledWebpagesTask) && x.Enabled);

            return !any
                ? new HealthCheckResult
                {
                    Messages = new List<string>
                    {
                        "Publisher task is not set up."
                    }
                }
                : HealthCheckResult.Success;
        }
    }
}