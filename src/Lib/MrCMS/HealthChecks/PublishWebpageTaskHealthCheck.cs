using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Tasks;
using NHibernate;

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

        public override HealthCheckResult PerformCheck()
        {
            var any = _taskSettingManager.GetInfo().Any(x => x.Type == typeof(PublishScheduledWebpagesTask) && x.Enabled);

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