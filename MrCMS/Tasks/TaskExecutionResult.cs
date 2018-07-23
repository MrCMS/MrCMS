using System;

namespace MrCMS.Tasks
{
    public class TaskExecutionResult
    {
        private TaskExecutionResult()
        {
        }

        public bool Success { get; set; }
        public Exception Exception { get; set; }
        public AdHocTask Task { get; set; }

        public static TaskExecutionResult Successful(AdHocTask task)
        {
            return new TaskExecutionResult {Success = true, Task = task};
        }

        public static TaskExecutionResult Failure(AdHocTask task, Exception exception)
        {
            return new TaskExecutionResult {Success = false, Task = task, Exception = exception};
        }
    }
}