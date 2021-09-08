using System.Collections.Generic;

namespace MrCMS.Tasks
{
    public class BatchExecutionResult
    {
        public IReadOnlyCollection<TaskExecutionResult> Results { get; set; }
    }
}