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

        public void Execute(BatchRunStartArgs args)
        {
            _executeRequestForNextTask.Execute(args.BatchRun);
        }
    }
}