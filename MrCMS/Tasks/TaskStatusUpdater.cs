using System;
using System.Collections.Generic;
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

        public void BeginExecution(IExecutableTask executableTask)
        {
            SetStatus(executableTask, status => status.OnStarting());
        }

        public void BeginExecution(IEnumerable<IExecutableTask> executableTasks)
        {
            SetStatus(executableTasks, status => status.OnStarting());
        }

        public void SuccessfulCompletion(IExecutableTask executableTask)
        {
            SetStatus(executableTask, status => status.OnSuccess());
        }

        public void SuccessfulCompletion(IEnumerable<IExecutableTask> executableTasks)
        {
            SetStatus(executableTasks, status => status.OnSuccess());
        }

        public void FailedExecution(IExecutableTask executableTask)
        {
            SetStatus(executableTask, status => status.OnFailure());
        }

        public void FailedExecution(IEnumerable<IExecutableTask> executableTasks)
        {
            SetStatus(executableTasks, status => status.OnFailure());
        }

        private void SetStatus(IExecutableTask executableTask, Action<IHaveExecutionStatus> action)
        {
            _session.Transact(session =>
                                  {
                                      action(executableTask.Entity);
                                      session.Update(executableTask.Entity);
                                  });
        }
        private void SetStatus(IEnumerable<IExecutableTask> executableTasks, Action<IHaveExecutionStatus> action)
        {
            _session.Transact(session => executableTasks.ForEach(task => SetStatus(task, action)));
        }
    }
}