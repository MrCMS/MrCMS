using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<List<TaskInfo>> GetTasks()
        {
            var startTime = await _getDateTimeNow.GetLocalNow();
            var info = await _taskSettingManager.GetInfo();
            var scheduledTasks =
                info
                    .Where(task =>
                        task.Enabled && task.Status == TaskExecutionStatus.Pending &&
                        (task.LastCompleted < startTime.AddSeconds(-task.FrequencyInSeconds) ||
                         task.LastCompleted == null))
                    .ToList();
            await _taskSettingManager.StartTasks(scheduledTasks, startTime);
            return scheduledTasks;
        }
    }
}