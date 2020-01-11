using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task<List<TaskExecutionResult>> ExecuteTasks(IList<AdHocTask> list, CancellationToken token)
        {
            await _taskStatusUpdater.BeginExecution(list);
            var actions = (await Task.WhenAll(list.Select(task => task as ILuceneIndexTask)
                .Select(task => task.GetActions(token)))).SelectMany(x => x);
            List<LuceneAction> luceneActions = actions
                .Distinct(LuceneActionComparison.Comparer)
                .ToList();

            LuceneActionExecutor.PerformActions(_indexService, luceneActions);
            List<TaskExecutionResult> results = list.Select(TaskExecutionResult.Successful).ToList();
            await _taskStatusUpdater.CompleteExecution(results);
            return results;
        }
    }
}