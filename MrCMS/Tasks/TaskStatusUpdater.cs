using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Tasks
{
    public class TaskStatusUpdater : ITaskStatusUpdater
    {
        private readonly ISession _session;

        public TaskStatusUpdater(ISession session)
        {
            _session = session;
        }

        public void BeginExecution(IEnumerable<IExecutableTask> executableTasks)
        {
            SetStatus(executableTasks, (status, task) => status.OnStarting(task));
        }

        public void CompleteExecution(IEnumerable<TaskExecutionResult> results)
        {
            IList<TaskExecutionResult> taskExecutionResults = results as IList<TaskExecutionResult> ?? results.ToList();
            _session.Transact(session =>
                              {
                                  SuccessfulCompletion(taskExecutionResults.Where(result => result.Success));
                                  FailedExecution(taskExecutionResults.Where(result => !result.Success));
                              });
        }

        private void SuccessfulCompletion(IEnumerable<TaskExecutionResult> executableTasks)
        {
            SetStatus(executableTasks.Select(result => result.Task), (status, task) => status.OnSuccess(task));
        }

        private void FailedExecution(IEnumerable<TaskExecutionResult> taskFailureInfos)
        {
            _session.Transact(session => taskFailureInfos.ForEach(
                taskFailureInfo =>
                {
                    IExecutableTask executableTask = taskFailureInfo.Task;
                    executableTask.Entity.OnFailure(executableTask, taskFailureInfo.Exception);
                    session.Update(executableTask.Entity);
                }));
        }

        private void SetStatus(IEnumerable<IExecutableTask> executableTasks,
            Action<IHaveExecutionStatus, IExecutableTask> action)
        {
            _session.Transact(session => executableTasks.ForEach(task =>
                                                                 {
                                                                     action(task.Entity, task);
                                                                     session.Update(task.Entity);
                                                                 }));
        }
    }
}