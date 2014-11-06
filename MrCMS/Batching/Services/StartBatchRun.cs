using MrCMS.Batching.Entities;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Batching.Services
{
    public class StartBatchRun : IStartBatchRun
    {
        private readonly ISession _session;

        public StartBatchRun(ISession session)
        {
            _session = session;
        }

        public void Start(BatchRun batchRun)
        {
            if (batchRun == null || (batchRun.Status == BatchRunStatus.Executing || batchRun.Status == BatchRunStatus.Complete))
                return;
            _session.Transact(session =>
            {
                batchRun.Status = BatchRunStatus.Executing;
                session.Update(batchRun);
            });
        }
    }
}