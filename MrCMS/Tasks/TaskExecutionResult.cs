using System;

namespace MrCMS.Tasks
{
    public class TaskExecutionResult
    {
        public bool Success { get; set; }
        public Exception Exception{ get; set; }
    }
}