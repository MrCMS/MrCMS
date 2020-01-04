using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MrCMS.Tasks;

namespace MrCMS.Website.Filters
{
    public class ExecuteLuceneTasksRunner : ExecuteEndRequestBase<ExecuteLuceneTasks, int>
    {
        private readonly IQueuedTaskRunner _queuedTaskRunner;

        public ExecuteLuceneTasksRunner(IQueuedTaskRunner queuedTaskRunner)
        {
            _queuedTaskRunner = queuedTaskRunner;
        }

        public override async Task Execute(IEnumerable<int> data, CancellationToken token)
        {
            await _queuedTaskRunner.ExecuteLuceneTasks(token);
        }
    }
}