using System.Collections.Generic;
using System.Linq;
using MrCMS.Website;

namespace MrCMS.Tasks
{
    public class GetPendingScheduledTasks : IGetPendingScheduledTasks
    {
        private readonly IGetDateTimeNow _getDateTimeNow;
        private readonly ITaskSettingManager _taskSettingManager;

        public GetPendingScheduledTasks(IGetDateTimeNow getDateTimeNow, ITaskSettingManager taskSettingManager)
        {
            _getDateTimeNow = getDateTimeNow;
            _taskSettingManager = taskSettingManager;
        }

        public List<TaskInfo> GetTasks()
        {
            var startTime = _getDateTimeNow.LocalNow;
            var scheduledTasks =
                _taskSettingManager.GetInfo()
                    .Where(task =>
                        task.Enabled && task.Status == TaskExecutionStatus.Pending &&
                        (task.LastCompleted < startTime.AddSeconds(-task.FrequencyInSeconds) ||
                         task.LastCompleted == null))
                    .ToList();
            _taskSettingManager.StartTasks(scheduledTasks, startTime);
            return scheduledTasks;
        }
    }
}