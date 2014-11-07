using MrCMS.Batching.Entities;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Batching.Services
{
    public class ControlBatchRun : IControlBatchRun
    {
        private readonly ISession _session;

        public ControlBatchRun(ISession session)
        {
            _session = session;
        }

        public bool Start(BatchRun batchRun)
        {
            if (batchRun == null ||
                (batchRun.Status == BatchRunStatus.Executing || batchRun.Status == BatchRunStatus.Complete))
                return false;
            _session.Transact(session =>
            {
                batchRun.Status = BatchRunStatus.Executing;
                session.Update(batchRun);
            });
            return true;
        }

        public bool Pause(BatchRun batchRun)
        {
            if (batchRun == null ||
                (batchRun.Status != BatchRunStatus.Executing))
                return false;
            _session.Transact(session =>
            {
                batchRun.Status = BatchRunStatus.Paused;
                session.Update(batchRun);
            });
            return true;
        }
    }
}