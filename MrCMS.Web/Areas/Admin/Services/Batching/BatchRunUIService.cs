using System.Collections.Generic;
using MrCMS.Batching.Entities;
using MrCMS.Batching.Services;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services.Batching
{
    public class BatchRunUIService : IBatchRunUIService
    {
        private readonly ISession _session;
        private readonly IControlBatchRun _controlBatchRun;
        private readonly IExecuteNextBatchJob _executeNextBatchJob;

        public BatchRunUIService(ISession session, IControlBatchRun controlBatchRun, IExecuteNextBatchJob executeNextBatchJob)
        {
            _session = session;
            _controlBatchRun = controlBatchRun;
            _executeNextBatchJob = executeNextBatchJob;
        }

        public IList<BatchRunResult> GetResults(BatchRun batchRun)
        {
            if (batchRun == null)
                return new List<BatchRunResult>();

            return _session.QueryOver<BatchRunResult>()
                .Where(result => result.BatchRun.Id == batchRun.Id)
                .OrderBy(result => result.ExecutionOrder)
                .Asc.Cacheable()
                .List();
        }

        public int? Start(BatchRun run)
        {
            return _controlBatchRun.Start(run) ? run.Id : (int?) null;
        }

        public bool Pause(BatchRun run)
        {
            return _controlBatchRun.Pause(run);
        }

        public int? ExecuteNextTask(BatchRun run)
        {
            return _executeNextBatchJob.Execute(run) ? run.Id : (int?) null;
        }
    }
}