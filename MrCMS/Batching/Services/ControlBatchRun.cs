using MrCMS.Batching.Entities;
using MrCMS.Batching.Events;
using MrCMS.Helpers;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Batching.Services
{
    public class ControlBatchRun : IControlBatchRun
    {
        private readonly ISession _session;
        private readonly IEventContext _eventContext;

        public ControlBatchRun(ISession session, IEventContext eventContext)
        {
            _session = session;
            _eventContext = eventContext;
        }

        public bool Start(BatchRun batchRun)
        {
            batchRun = GetBatchRunFromThisSession(batchRun);
            if (batchRun == null ||
                (batchRun.Status == BatchRunStatus.Executing || batchRun.Status == BatchRunStatus.Complete))
                return false;
            _session.Transact(session =>
            {
                batchRun.Status = BatchRunStatus.Executing;
                session.Update(batchRun);
            });
            _session.GetService<IEventContext>().Publish<IOnBatchRunStart, BatchRunStartArgs>(new BatchRunStartArgs
            {
                BatchRun = batchRun
            });
            return true;
        }

        public bool Pause(BatchRun batchRun)
        {
            batchRun = GetBatchRunFromThisSession(batchRun);
            if (batchRun == null ||
                (batchRun.Status != BatchRunStatus.Executing))
                return false;
            _session.Transact(session =>
            {
                batchRun.Status = BatchRunStatus.Paused;
                session.Update(batchRun);
            });
            _eventContext.Publish<IOnBatchRunPause, BatchRunPauseArgs>(new BatchRunPauseArgs
            {
                BatchRun = batchRun
            });
            return true;
        }

        private BatchRun GetBatchRunFromThisSession(BatchRun batchRun)
        {
            return _session.Get<BatchRun>(batchRun.Id);
        }
    }
}