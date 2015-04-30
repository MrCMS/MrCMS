using System.Collections.Generic;
using MrCMS.Tasks;

namespace MrCMS.Website.Filters
{
    public class ExecuteLuceneTasksRunner : ExecuteEndRequestBase<ExecuteLuceneTasks, int>
    {
        private readonly ITaskRunner _taskRunner;

        public ExecuteLuceneTasksRunner(ITaskRunner taskRunner)
        {
            _taskRunner = taskRunner;
        }

        public override void Execute(IEnumerable<int> data)
        {
            _taskRunner.ExecuteLuceneTasks();
        }
    }
}