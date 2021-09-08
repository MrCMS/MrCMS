using System.Threading.Tasks;
using MrCMS.Batching.Entities;
using MrCMS.Entities;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Batching
{
    public class SetBatchJobExecutionStatus : ISetBatchJobExecutionStatus
    {
        private readonly ISession _session;

        public SetBatchJobExecutionStatus(ISession session)
        {
            _session = session;
        }

        public async Task Starting<T>(T entity) where T : SystemEntity, IHaveJobExecutionStatus
        {
            await _session.TransactAsync(async session =>
            {
                entity.Status = JobExecutionStatus.Executing;
                await session.UpdateAsync(entity);
            });
        }

        public async Task Complete<T>(T entity, BatchJobExecutionResult result) where T : SystemEntity, IHaveJobExecutionStatus
        {
            await _session.TransactAsync(async session =>
            {
                entity.Status = result.Successful ? JobExecutionStatus.Succeeded : JobExecutionStatus.Failed;
                await session.UpdateAsync(entity);
            });
        }
    }
}