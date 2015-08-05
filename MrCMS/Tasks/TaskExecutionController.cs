using System.Web.Mvc;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Tasks
{
    public class TaskExecutionController : MrCMSUIController
    {
        public const string ExecutePendingTasksURL = "execute-pending-tasks";
        private readonly ITaskRunner _taskRunner;

        public TaskExecutionController(ITaskRunner taskRunner)
        {
            _taskRunner = taskRunner;
        }

        [TaskExecutionKeyPasswordAuth]
        public ContentResult Execute()
        {
            BatchExecutionResult result = _taskRunner.ExecutePendingTasks();
            return new ContentResult {Content = "Executed", ContentType = "text/plain"};
        }
    }
}