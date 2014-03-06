using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Multisite;
using MrCMS.Services;
using WebGrease.Css.Extensions;

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

        public int Priority { get { return 100; } }

        public IList<IExecutableTask> ExtractTasksToHandle(ref IList<IExecutableTask> list)
        {
            var executableTasks = list.Where(task => task is ILuceneIndexTask).ToList();
            foreach (var executableTask in executableTasks)
                list.Remove(executableTask);
            return executableTasks;
        }

        public List<TaskExecutionResult> ExecuteTasks(IList<IExecutableTask> list)
        {
            list.ForEach(task => _taskStatusUpdater.BeginExecution(task));
            var luceneActions =
                list.Select(task => task as ILuceneIndexTask)
                    .SelectMany(task => task.GetActions())
                    .Distinct(LuceneActionComparison.Comparer)
                    .ToList();

            LuceneActionExecutor.PerformActions(_indexService, luceneActions);
            list.ForEach(task => _taskStatusUpdater.SuccessfulCompletion(task));
            return new List<TaskExecutionResult> { new TaskExecutionResult { Success = true } };
        }
    }
}