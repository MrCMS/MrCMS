using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Tasks;

namespace MrCMS.HealthChecks
{
    public class EmailSenderTaskCheck : HealthCheck
    {
        private readonly ITaskSettingManager _taskSettingManager;

        public EmailSenderTaskCheck(ITaskSettingManager taskSettingManager)
        {
            _taskSettingManager = taskSettingManager;
        }

        public override string DisplayName => "Send email task setup";

        public override async Task<HealthCheckResult> PerformCheck()
        {
            var info = await _taskSettingManager.GetInfo();
            var any = info.Any(x => x.Type == typeof (SendQueuedMessagesTask) && x.Enabled);

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