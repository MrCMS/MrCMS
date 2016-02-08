using System;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Tasks;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.HealthChecks
{
    public class TaskRunnerCheck : HealthCheck
    {
        private readonly ITaskSettingManager _taskSettingManager;

        public TaskRunnerCheck(ITaskSettingManager taskSettingManager)
        {
            _taskSettingManager = taskSettingManager;
        }

        public override string DisplayName
        {
            get { return "Task Runner"; }
        }

        public override HealthCheckResult PerformCheck()
        {
            var tasks = _taskSettingManager.GetInfo().ToList().Where(x =>
            {
                return x.LastCompleted != null &&
                       (x.Enabled &&
                        x.LastCompleted.Value <= CurrentRequestData.Now.AddSeconds(-(x.FrequencyInSeconds + 120)) ||
                        x.LastCompleted == null);
            });

            if (tasks.Any())
            {
                var messages = tasks.Select(task =>
                {
                    var lastComplete = task.LastCompleted;
                    return lastComplete.HasValue
                        ? string.Format("{0} has not been ran since {1}", task.Name, lastComplete)
                        : string.Format("{0} has never been run", task.TypeName);
                }).ToList();
                return new HealthCheckResult
                {
                    Messages = messages,
                    OK = false
                };
            }
            return HealthCheckResult.Success;
        }
    }
}