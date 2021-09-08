using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Tasks
{
    public class TaskExecutionController : MrCMSUIController
    {
        public const string ExecuteTaskURL = "execute-task";
        public const string ExecuteQueuedTasksURL = "execute-queued-tasks";
        private readonly IQueuedTaskRunner _queuedTaskRunner;
        private readonly ITaskResetter _taskResetter;
        private readonly ISiteTaskRunner _siteTaskRunner;
        private readonly ILogger<TaskExecutionController> _logger;

        public TaskExecutionController(IQueuedTaskRunner queuedTaskRunner,
            ITaskResetter taskResetter,
            ISiteTaskRunner siteTaskRunner,
            ILogger<TaskExecutionController> logger)
        {
            _queuedTaskRunner = queuedTaskRunner;
            _taskResetter = taskResetter;
            _siteTaskRunner = siteTaskRunner;
            _logger = logger;
        }


        [TaskExecutionKeyPasswordAuth]
        [Route(ExecuteTaskURL)]
        public async Task<ContentResult> ExecuteTask(string type)
        {
            await _siteTaskRunner.ExecuteTask(type);
            return new ContentResult {Content = "Executed", ContentType = "text/plain"};
        }

        [TaskExecutionKeyPasswordAuth]
        [Route(ExecuteQueuedTasksURL)]
        public async Task<ContentResult> ExecuteQueuedTasks()
        {
            _logger.LogInformation("Handling Queued Tasks");
            await _taskResetter.ResetHungTasks();
            await _queuedTaskRunner.ExecutePendingTasks();
            return new ContentResult {Content = "Executed", ContentType = "text/plain"};
        }
    }
}