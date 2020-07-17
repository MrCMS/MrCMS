using MrCMS.Batching.Entities;

namespace MrCMS.Web.Admin.Services.Batching
{
    public interface IGetBatchStatus
    {
        BatchStatus Get(Batch batch);
    }
}