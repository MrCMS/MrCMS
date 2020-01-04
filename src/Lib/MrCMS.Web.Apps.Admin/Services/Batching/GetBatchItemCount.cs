using MrCMS.Batching.Entities;


namespace MrCMS.Web.Apps.Admin.Services.Batching
{
    public class GetBatchItemCount : IGetBatchItemCount
    {
        private readonly ISession _session;

        public GetBatchItemCount(ISession session)
        {
            _session = session;
        }

        public int Get(Batch batch)
        {
            return batch == null
                ? 0
                : _session.QueryOver<BatchJob>().Where(job => job.Batch.Id == batch.Id).RowCount();
        }
    }
}