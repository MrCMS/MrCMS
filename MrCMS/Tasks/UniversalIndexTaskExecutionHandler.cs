using System.Collections.Generic;
using System.Linq;
using MrCMS.Search;

namespace MrCMS.Tasks
{
    public class UniversalIndexTaskExecutionHandler : ITaskExecutionHandler
    {
        private readonly IUniversalSearchIndexManager _universalSearchIndexManager;
        private readonly ISearchConverter _searchConverter;
        private readonly ITaskStatusUpdater _taskStatusUpdater;

        public UniversalIndexTaskExecutionHandler(IUniversalSearchIndexManager universalSearchIndexManager,ISearchConverter searchConverter, ITaskStatusUpdater taskStatusUpdater)
        {
            _universalSearchIndexManager = universalSearchIndexManager;
            _searchConverter = searchConverter;
            _taskStatusUpdater = taskStatusUpdater;
        }

        public int Priority
        {
            get { return 200; }
        }

        public IList<IExecutableTask> ExtractTasksToHandle(ref IList<IExecutableTask> list)
        {
            List<IExecutableTask> executableTasks = list.Where(task => task is IUniversalSearchIndexTask).ToList();
            foreach (IExecutableTask executableTask in executableTasks)
                list.Remove(executableTask);
            return executableTasks;
        }

        public List<TaskExecutionResult> ExecuteTasks(IList<IExecutableTask> list)
        {
            _taskStatusUpdater.BeginExecution(list);
            List<UniversalSearchIndexData> data =
                list.Select(task => task as IUniversalSearchIndexTask)
                    .Select(task => task.UniversalSearchIndexData)
                    .Distinct(UniversalSearchIndexData.Comparer)
                    .ToList();

            UniversalSearchActionExecutor.PerformActions(_universalSearchIndexManager, _searchConverter, data);
            List<TaskExecutionResult> results = list.Select(TaskExecutionResult.Successful).ToList();
            _taskStatusUpdater.CompleteExecution(results);
            return results;
        }
    }
}