using System.Linq;
using MrCMS.Batching.Entities;
using MrCMS.Data;

namespace MrCMS.Web.Areas.Admin.Services.Batching
{
    public class GetBatchItemCount : IGetBatchItemCount
    {
        private readonly IRepository<BatchJob> _repository;

        public GetBatchItemCount(IRepository<BatchJob> repository)
        {
            _repository = repository;
        }


        public int Get(Batch batch)
        {
            return batch == null
                ? 0
                : _repository.Readonly().Count(job => job.Batch.Id == batch.Id);
        }
    }
}