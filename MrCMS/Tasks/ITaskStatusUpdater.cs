using System;
using System.Collections.Generic;

namespace MrCMS.Tasks
{
    public interface ITaskStatusUpdater
    {
        void BeginExecution(IEnumerable<IExecutableTask> executableTasks);
        void CompleteExecution(IEnumerable<TaskExecutionResult> results);
        //void SuccessfulCompletion(IEnumerable<IExecutableTask> executableTasks);
        //void FailedExecution(IEnumerable<ITaskFailureInfo> taskFailureInfos);
    }
}