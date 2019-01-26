using Microsoft.AspNetCore.Mvc;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Tasks
{
    public class TaskExecutionController : MrCMSUIController
    {
        public const string ExecutePendingTasksURL = "execute-pending-tasks";
        public const string ExecuteTaskURL = "execute-task";
        public const string ExecuteQueuedTasksURL = "execute-queued-tasks";
        private readonly IQueuedTaskRunner _queuedTaskRunner;
        private readonly ITaskResetter _taskResetter;
        private readonly ITriggerScheduledTasks _triggerScheduledTasks;
        private readonly IScheduledTaskRunner _scheduledTaskRunner;

        public TaskExecutionController(IQueuedTaskRunner queuedTaskRunner, ITaskResetter taskResetter,
            ITriggerScheduledTasks triggerScheduledTasks, IScheduledTaskRunner scheduledTaskRunner)
        {
            _queuedTaskRunner = queuedTaskRunner;
            _taskResetter = taskResetter;
            _triggerScheduledTasks = triggerScheduledTasks;
            _scheduledTaskRunner = scheduledTaskRunner;
        }

        [TaskExecutionKeyPasswordAuth]
        [Route(ExecutePendingTasksURL)]
        public ContentResult Execute()
        {
            _taskResetter.ResetHungTasks();
            _triggerScheduledTasks.Trigger();
            _queuedTaskRunner.TriggerPendingTasks();
            return new ContentResult {Content = "Executed", ContentType = "text/plain"};
        }

        [TaskExecutionKeyPasswordAuth]
        [Route(ExecuteTaskURL)]
        public ContentResult ExecuteTask(string type)
        {
            _scheduledTaskRunner.ExecuteTask(type);
            return new ContentResult {Content = "Executed", ContentType = "text/plain"};
        }

        [TaskExecutionKeyPasswordAuth]
        [Route(ExecuteQueuedTasksURL)]
        public ContentResult ExecuteQueuedTasks()
        {
            _queuedTaskRunner.ExecutePendingTasks();
            return new ContentResult {Content = "Executed", ContentType = "text/plain"};
        }
    }
}