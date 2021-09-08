using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Tasks
{
    public class TaskStatusUpdater : ITaskStatusUpdater
    {
        private readonly ISession _session;

        public TaskStatusUpdater(ISession session)
        {
            _session = session;
        }

        public async Task BeginExecution(IEnumerable<AdHocTask> executableTasks)
        {
            await SetStatus(executableTasks, (status, task) => status.OnStarting(task));
        }

        public async Task CompleteExecution(IEnumerable<TaskExecutionResult> results)
        {
            IList<TaskExecutionResult> taskExecutionResults = results as IList<TaskExecutionResult> ?? results.ToList();
            await _session.TransactAsync(async session =>
            {
                await SuccessfulCompletion(taskExecutionResults.Where(result => result.Success));
                await FailedExecution(taskExecutionResults.Where(result => !result.Success));
            });
        }

        private async Task SuccessfulCompletion(IEnumerable<TaskExecutionResult> executableTasks)
        {
            await SetStatus(executableTasks.Select(result => result.Task), (status, task) => status.OnSuccess(task));
        }

        private async Task FailedExecution(IEnumerable<TaskExecutionResult> taskFailureInfos)
        {
            await _session.TransactAsync(async session =>
            {
                foreach (var taskFailureInfo in taskFailureInfos)
                {
                    AdHocTask executableTask = taskFailureInfo.Task;
                    executableTask.Entity.OnFailure(executableTask, taskFailureInfo.Exception);
                    await session.UpdateAsync(executableTask.Entity);
                }
            });
        }

        private async Task SetStatus(IEnumerable<AdHocTask> executableTasks,
            Action<IHaveExecutionStatus, AdHocTask> action)
        {
            await _session.TransactAsync(async session =>
            {
                foreach (var task in executableTasks)
                {
                    action(task.Entity, task);
                    await session.UpdateAsync(task.Entity);
                }
            });
        }
    }
}