using System.Threading.Tasks;
using MrCMS.Batching.Entities;
using MrCMS.Data;
using MrCMS.Helpers;

namespace MrCMS.Batching.Services
{
    public class SetRunStatus : ISetRunStatus
    {
        private readonly IRepository<BatchRun> _repository;

        public SetRunStatus(IRepository<BatchRun> repository)
        {
            _repository = repository;
        }


        public async Task Complete(BatchRun batchRun)
        {
            await SetStatus(batchRun, BatchRunStatus.Complete);
        }

        public async Task Paused(BatchRun batchRun)
        {
            await SetStatus(batchRun, BatchRunStatus.Paused);
        }

        private Task SetStatus(BatchRun batchRun, BatchRunStatus status)
        {
            batchRun.Status = status;
            return _repository.Update(batchRun);
        }
    }
}