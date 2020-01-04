using System.Threading.Tasks;
using MrCMS.Batching.Entities;
using MrCMS.Batching.Events;
using MrCMS.Data;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Batching.Services
{
    public class ControlBatchRun : IControlBatchRun
    {
        private readonly IRepository<BatchRun> _repository;
        private readonly IEventContext _eventContext;

        public ControlBatchRun(IEventContext eventContext, IRepository<BatchRun> repository)
        {
            _eventContext = eventContext;
            _repository = repository;
        }

        public async Task<bool> Start(BatchRun batchRun)
        {
            batchRun = await GetBatchRunFromThisSession(batchRun);
            if (batchRun == null ||
                (batchRun.Status == BatchRunStatus.Executing || batchRun.Status == BatchRunStatus.Complete))
                return false;
            batchRun.Status = BatchRunStatus.Executing;
            await _repository.Update(batchRun);
            _eventContext.Publish<IOnBatchRunStart, BatchRunStartArgs>(new BatchRunStartArgs
            {
                BatchRun = batchRun
            });
            return true;
        }

        public async Task<bool> Pause(BatchRun batchRun)
        {
            batchRun = await GetBatchRunFromThisSession(batchRun);
            if (batchRun == null ||
                (batchRun.Status != BatchRunStatus.Executing))
                return false;
            batchRun.Status = BatchRunStatus.Paused;
            await _repository.Update(batchRun);
            _eventContext.Publish<IOnBatchRunPause, BatchRunPauseArgs>(new BatchRunPauseArgs
            {
                BatchRun = batchRun
            });
            return true;
        }

        private Task<BatchRun> GetBatchRunFromThisSession(BatchRun batchRun)
        {
            return _repository.Load(batchRun.Id);
        }
    }
}