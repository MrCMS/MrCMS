using MrCMS.Batching.Entities;

namespace MrCMS.Web.Apps.Admin.Services.Batching
{
    public interface IGetBatchItemCount
    {
        int Get(Batch batch);
    }
}