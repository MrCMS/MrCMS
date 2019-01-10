using MrCMS.Batching.Entities;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Batching.Services
{
    public class SetRunStatus : ISetRunStatus
    {
        private readonly ISession _session;

        public SetRunStatus(ISession session)
        {
            _session = session;
        }

        public void Complete(BatchRun batchRun)
        {
            SetStatus(batchRun, BatchRunStatus.Complete);
        }

        public void Paused(BatchRun batchRun)
        {
            SetStatus(batchRun, BatchRunStatus.Paused);
        }

        private void SetStatus(BatchRun batchRun, BatchRunStatus status)
        {
            _session.Transact(session =>
            {
                batchRun.Status = status;
                session.Update(batchRun);
            });
        }
    }
}