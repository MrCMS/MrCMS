using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Website;

namespace MrCMS.Tasks
{
    public class TaskResetter : ITaskResetter
    {
        private readonly IGlobalRepository<QueuedTask> _repository;
        private readonly ITaskSettingManager _taskSettingManager;
        private readonly IGetDateTimeNow _getDateTimeNow;

        public TaskResetter(IGlobalRepository<QueuedTask> repository, ITaskSettingManager taskSettingManager, IGetDateTimeNow getDateTimeNow)
        {
            _repository = repository;
            _taskSettingManager = taskSettingManager;
            _getDateTimeNow = getDateTimeNow;
        }

        public async Task ResetHungTasks()
        {
            await ResetQueuedTasks();
            await ResetScheduledTasks();
        }

        private async Task ResetScheduledTasks()
        {
            var now = _getDateTimeNow.LocalNow;
            var info = await _taskSettingManager.GetInfo();
            var hungScheduledTasks = info
                .Where(
                    task => task.Enabled &&
                        (task.Status == TaskExecutionStatus.AwaitingExecution ||
                         task.Status == TaskExecutionStatus.Executing || task.Status == TaskExecutionStatus.Failed) &&
                        (task.LastStarted < now.AddMinutes(-15) || task.LastStarted == null)
                )
                .ToList();
            foreach (var task in hungScheduledTasks)
            {
                await _taskSettingManager.Reset(task.Type, false);
            }
        }

        private async Task ResetQueuedTasks()
        {
            var now = _getDateTimeNow.LocalNow;
            var hungTasks = await _repository.Query()
                .Where(
                    task =>
                        (task.Status == TaskExecutionStatus.AwaitingExecution ||
                         task.Status == TaskExecutionStatus.Executing) &&
                        task.QueuedAt < now.AddMinutes(-15))
                .ToListAsync();
            foreach (var task in hungTasks)
            {
                task.QueuedAt = null;
                task.Status = TaskExecutionStatus.Pending;
            }

            await _repository.UpdateRange(hungTasks);
        }
    }
}