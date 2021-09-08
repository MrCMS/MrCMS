using System.Threading.Tasks;
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

        public async Task Complete(BatchRun batchRun)
        {
            await SetStatus(batchRun, BatchRunStatus.Complete);
        }

        public async Task Paused(BatchRun batchRun)
        {
            await SetStatus(batchRun, BatchRunStatus.Paused);
        }

        private async Task SetStatus(BatchRun batchRun, BatchRunStatus status)
        {
            await _session.TransactAsync(async session =>
            {
                batchRun.Status = status;
                await session.UpdateAsync(batchRun);
            });
        }
    }
}