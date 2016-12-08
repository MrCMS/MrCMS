using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Tasks;
using NHibernate;

namespace MrCMS.HealthChecks
{
    public class EmailSenderTaskCheck : HealthCheck
    {
        private readonly ITaskSettingManager _taskSettingManager;

        public EmailSenderTaskCheck(ITaskSettingManager taskSettingManager)
        {
            _taskSettingManager = taskSettingManager;
        }

        public override string DisplayName
        {
            get { return "Send email task setup"; }
        }

        public override HealthCheckResult PerformCheck()
        {
            var any = _taskSettingManager.GetInfo().Any(x => x.Type == typeof (SendQueuedMessagesTask) && x.Enabled);

            return !any
                ? new HealthCheckResult
                {
                    Messages = new List<string>
                    {
                        "Email sending task is not set up."
                    }
                }
                : HealthCheckResult.Success;
        }
    }
}