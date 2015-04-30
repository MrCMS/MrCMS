using MrCMS.Batching.Entities;

namespace MrCMS.Web.Areas.Admin.Services.Batching
{
    public interface IGetBatchStatus
    {
        BatchStatus Get(Batch batch);
    }
}