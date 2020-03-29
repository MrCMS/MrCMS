using System.Threading.Tasks;
using MrCMS.Batching;
using MrCMS.Batching.Events;

namespace MrCMS.Web.Areas.Admin.Events
{
    public class StartBatchExecutionOnServer : IOnBatchRunStart
    {
        private readonly IExecuteRequestForNextTask _executeRequestForNextTask;

        public StartBatchExecutionOnServer(IExecuteRequestForNextTask executeRequestForNextTask)
        {
            _executeRequestForNextTask = executeRequestForNextTask;
        }

        public Task Execute(BatchRunStartArgs args)
        {
            return _executeRequestForNextTask.Execute(args.BatchRun);
        }
    }
}