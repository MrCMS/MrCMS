using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Helpers;

namespace MrCMS.Tasks
{
    public class TaskStatusUpdater : ITaskStatusUpdater
    {
        private readonly IRepository<QueuedTask> _taskRepository;

        public TaskStatusUpdater(IRepository<QueuedTask> taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task BeginExecution(IEnumerable<AdHocTask> executableTasks)
        {
            await SetStatus(executableTasks.ToList(), (status, task) => status.OnStarting(task));
        }

        public async Task CompleteExecution(IEnumerable<TaskExecutionResult> results)
        {
            IList<TaskExecutionResult> taskExecutionResults = results as IList<TaskExecutionResult> ?? results.ToList();
            await _taskRepository.Transact(async (repo, ct) =>
                              {
                                  await SuccessfulCompletion(taskExecutionResults.Where(result => result.Success));
                                  await FailedExecution(taskExecutionResults.Where(result => !result.Success).ToList());
                              });
        }

        private async Task SuccessfulCompletion(IEnumerable<TaskExecutionResult> executableTasks)
        {
            await SetStatus(executableTasks.Select(result => result.Task).ToList(), (status, task) => status.OnSuccess(task));
        }

        private async Task FailedExecution(ICollection<TaskExecutionResult> taskFailureInfos)
        {
            await _taskRepository.Transact(async (repo, ct) =>
            {
                taskFailureInfos.ForEach(
                    taskFailureInfo =>
                    {
                        AdHocTask executableTask = taskFailureInfo.Task;
                        executableTask.Entity.OnFailure(executableTask, taskFailureInfo.Exception);
                    });
                await repo.UpdateRange(taskFailureInfos.Select(x => x.Task.Entity).ToList(), ct);
            });
        }

        private async Task SetStatus(ICollection<AdHocTask> executableTasks,
            Action<QueuedTask, AdHocTask> action)
        {
            await _taskRepository.Transact(async (repo, ct) =>
            {
                executableTasks.ForEach(task =>
                {
                    action(task.Entity, task);
                });
                await repo.UpdateRange(executableTasks.Select(x => x.Entity).ToList(), ct);
            });
        }
    }
}