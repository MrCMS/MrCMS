using System.Collections.Generic;
using System.Linq;
using MrCMS.Services;

namespace MrCMS.Tasks
{
    public class LuceneIndexTaskExecutionHandler : ITaskExecutionHandler
    {
        private readonly IIndexService _indexService;
        private readonly ITaskStatusUpdater _taskStatusUpdater;

        public LuceneIndexTaskExecutionHandler(IIndexService indexService, ITaskStatusUpdater taskStatusUpdater)
        {
            _indexService = indexService;
            _taskStatusUpdater = taskStatusUpdater;
        }

        public int Priority
        {
            get { return 100; }
        }

        public IList<AdHocTask> ExtractTasksToHandle(ref IList<AdHocTask> list)
        {
            List<AdHocTask> executableTasks = list.Where(task => task is ILuceneIndexTask).ToList();
            foreach (AdHocTask executableTask in executableTasks)
                list.Remove(executableTask);
            return executableTasks;
        }

        public List<TaskExecutionResult> ExecuteTasks(IList<AdHocTask> list)
        {
            _taskStatusUpdater.BeginExecution(list);
            List<LuceneAction> luceneActions =
                list.Select(task => task as ILuceneIndexTask)
                    .SelectMany(task => task.GetActions())
                    .Distinct(LuceneActionComparison.Comparer)
                    .ToList();

            LuceneActionExecutor.PerformActions(_indexService, luceneActions);
            List<TaskExecutionResult> results = list.Select(TaskExecutionResult.Successful).ToList();
            _taskStatusUpdater.CompleteExecution(results);
            return results;
        }
    }
}