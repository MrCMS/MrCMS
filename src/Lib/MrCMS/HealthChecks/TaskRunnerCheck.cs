using System.Threading.Tasks;
using MrCMS.Tasks;
using MrCMS.Website;

namespace MrCMS.HealthChecks
{
    public class TaskRunnerCheck : HealthCheck
    {
        private readonly ITaskSettingManager _taskSettingManager;
        private readonly IGetDateTimeNow _getDateTimeNow;

        public TaskRunnerCheck(ITaskSettingManager taskSettingManager, IGetDateTimeNow getDateTimeNow)
        {
            _taskSettingManager = taskSettingManager;
            _getDateTimeNow = getDateTimeNow;
        }

        public override string DisplayName => "Task Runner";

        public override Task<HealthCheckResult> PerformCheck()
        {
            // var nowForSite = _getDateTimeNow.LocalNow;
            // var tasks = _taskSettingManager.GetInfo()
            //             .ToList()
            //             .Where(x => x.Enabled &&
            //                 (!x.LastCompleted.HasValue || x.LastCompleted.Value <= nowForSite.AddSeconds(-(x.FrequencyInSeconds + 120))));
            //
            // if (tasks.Any())
            // {
            //     var messages = tasks.Select(task =>
            //     {
            //         var lastComplete = task.LastCompleted;
            //         return lastComplete.HasValue
            //             ? $"{task.Name} has not been ran since {lastComplete}"
            //             : $"{task.Name} has never been run";
            //     }).ToList();
            //     return new HealthCheckResult
            //     {
            //         Messages = messages,
            //         Status = HealthCheckStatus.Failure
            //     };
            // }
            // todo - potentially remove
            return Task.FromResult(HealthCheckResult.Success);
        }
    }
}