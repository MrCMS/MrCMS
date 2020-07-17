using MrCMS.Batching.Entities;

namespace MrCMS.Web.Admin.Services.Batching
{
    public interface IGetBatchItemCount
    {
        int Get(Batch batch);
    }
}