using System.Threading.Tasks;
using MrCMS.Batching;
using MrCMS.Batching.Events;

namespace MrCMS.Web.Admin.Events
{
    public class StartBatchExecutionOnServer : IOnBatchRunStart
    {
        private readonly IExecuteRequestForNextTask _executeRequestForNextTask;

        public StartBatchExecutionOnServer(IExecuteRequestForNextTask executeRequestForNextTask)
        {
            _executeRequestForNextTask = executeRequestForNextTask;
        }

        public async Task Execute(BatchRunStartArgs args)
        {
            await _executeRequestForNextTask.Execute(args.BatchRun);
        }
    }
}