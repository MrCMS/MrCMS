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
        private readonly ITaskStatusUpdater _taskStatusUpdater;
        private readonly Site _site;

        public LuceneIndexTaskExecutionHandler(ITaskStatusUpdater taskStatusUpdater, Site site)
        {
            _taskStatusUpdater = taskStatusUpdater;
            _site = site;
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

            foreach (var @group in luceneActions.GroupBy(action => action.Type))
            {
                var managerBase = IndexService.GetIndexManagerBase(@group.Key, _site);

                IGrouping<Type, LuceneAction> thisGroup = @group;
                managerBase.Write(writer =>
                                      {
                                          foreach (var luceneAction in thisGroup.Where(action => action.Operation == LuceneOperation.Insert).ToList())
                                              luceneAction.Execute(writer);
                                          foreach (var luceneAction in thisGroup.Where(action => action.Operation == LuceneOperation.Update).ToList())
                                              luceneAction.Execute(writer);
                                          foreach (var luceneAction in thisGroup.Where(action => action.Operation == LuceneOperation.Delete).ToList())
                                              luceneAction.Execute(writer);
                                      });
            }
            list.ForEach(task => _taskStatusUpdater.SuccessfulCompletion(task));
            return new List<TaskExecutionResult> { new TaskExecutionResult { Success = true } };
        }
    }
}