using MrCMS.Website;
using NHibernate;
using MrCMS.Helpers;

namespace MrCMS.Tasks
{
    public class TaskRunner : ITaskRunner
    {
        private readonly ITaskResetter _taskResetter;
        private readonly ITaskQueuer _taskQueuer;
        private readonly ITaskBuilder _taskBuilder;
        private readonly ITaskExecutor _taskExecutor;

        public TaskRunner(ITaskResetter taskResetter, ITaskQueuer taskQueuer, ITaskBuilder taskBuilder, ITaskExecutor taskExecutor)
        {
            _taskResetter = taskResetter;
            _taskQueuer = taskQueuer;
            _taskBuilder = taskBuilder;
            _taskExecutor = taskExecutor;
        }

        public BatchExecutionResult ExecutePendingTasks()
        {
            _taskResetter.ResetHungTasks();

            var pendingQueuedTasks = _taskQueuer.GetPendingQueuedTasks();
            var pendingScheduledTasks = _taskQueuer.GetPendingScheduledTasks();

            var tasksToExecute = _taskBuilder.GetTasksToExecute(pendingQueuedTasks, pendingScheduledTasks);

            return _taskExecutor.Execute(tasksToExecute);
        }
    }

    public interface ITaskResetter
    {
        void ResetHungTasks();
    }

    public class TaskResetter : ITaskResetter
    {
        private readonly ISession _session;

        public TaskResetter(ISession session)
        {
            _session = session;
        }

        public void ResetHungTasks()
        {
            _session.Transact(session =>
                                  {
                                      var hungTasks = session.QueryOver<QueuedTask>()
                                                             .Where(
                                                                 task =>
                                                                 task.Status == TaskExecutionStatus.AwaitingExecution &&
                                                                 task.QueuedAt < CurrentRequestData.Now.AddMinutes(5))
                                                             .List();
                                      foreach (var task in hungTasks)
                                      {
                                          task.QueuedAt = null;
                                          task.Status = TaskExecutionStatus.Pending;
                                          session.Update(task);
                                      }
                                  });

        }
    }
}