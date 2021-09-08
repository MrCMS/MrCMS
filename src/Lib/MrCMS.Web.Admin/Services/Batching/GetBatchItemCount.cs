using System.Threading.Tasks;
using MrCMS.Batching.Entities;
using NHibernate;

namespace MrCMS.Web.Admin.Services.Batching
{
    public class GetBatchItemCount : IGetBatchItemCount
    {
        private readonly ISession _session;

        public GetBatchItemCount(ISession session)
        {
            _session = session;
        }

        public async Task<int> Get(Batch batch)
        {
            return batch == null
                ? 0
                : await _session.QueryOver<BatchJob>().Where(job => job.Batch.Id == batch.Id).RowCountAsync();
        }
    }
}