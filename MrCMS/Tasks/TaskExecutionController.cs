using System;
using System.Web.Mvc;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Tasks
{
    public class TaskExecutionController : MrCMSUIController
    {
        public const string ExecutePendingTasksURL = "execute-pending-tasks";
        public const string ExecuteTaskURL = "execute-task";
        private readonly IQueuedTaskRunner _queuedTaskRunner;
        private readonly ITaskResetter _taskResetter;
        private readonly IScheduledTaskRunner _scheduledTaskRunner;

        public TaskExecutionController(IQueuedTaskRunner queuedTaskRunner, ITaskResetter taskResetter, IScheduledTaskRunner scheduledTaskRunner)
        {
            _queuedTaskRunner = queuedTaskRunner;
            _taskResetter = taskResetter;
            _scheduledTaskRunner = scheduledTaskRunner;
        }

        [TaskExecutionKeyPasswordAuth]
        public ContentResult Execute()
        {
            _taskResetter.ResetHungTasks();
            _scheduledTaskRunner.TriggerScheduledTasks();
            BatchExecutionResult result = _queuedTaskRunner.ExecutePendingTasks();
            return new ContentResult {Content = "Executed", ContentType = "text/plain"};
        }

        [TaskExecutionKeyPasswordAuth]
        public ContentResult ExecuteTask(Guid id)
        {
            _scheduledTaskRunner.ExecuteTask(id);
            return new ContentResult {Content = "Executed", ContentType = "text/plain"};
        }
    }

}