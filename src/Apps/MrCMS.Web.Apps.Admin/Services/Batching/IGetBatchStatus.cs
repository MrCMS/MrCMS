using MrCMS.Batching.Entities;

namespace MrCMS.Web.Apps.Admin.Services.Batching
{
    public interface IGetBatchStatus
    {
        BatchStatus Get(Batch batch);
    }
}