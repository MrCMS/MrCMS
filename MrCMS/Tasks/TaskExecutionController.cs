using System.Web.Mvc;
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
        private readonly IScheduledTaskRunner _scheduledTaskRunner;
        private readonly ITaskResetter _taskResetter;

        public TaskExecutionController(IQueuedTaskRunner queuedTaskRunner, ITaskResetter taskResetter,
            IScheduledTaskRunner scheduledTaskRunner)
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
            _queuedTaskRunner.TriggerPendingTasks();
            return new ContentResult {Content = "Executed", ContentType = "text/plain"};
        }

        [TaskExecutionKeyPasswordAuth]
        public ContentResult ExecuteTask(string type)
        {
            _scheduledTaskRunner.ExecuteTask(type);
            return new ContentResult {Content = "Executed", ContentType = "text/plain"};
        }

        [TaskExecutionKeyPasswordAuth]
        public ContentResult ExecuteQueuedTasks()
        {
            _queuedTaskRunner.ExecutePendingTasks();
            return new ContentResult {Content = "Executed", ContentType = "text/plain"};
        }
    }
}