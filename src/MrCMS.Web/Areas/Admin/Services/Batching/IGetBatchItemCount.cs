using MrCMS.Batching.Entities;

namespace MrCMS.Web.Areas.Admin.Services.Batching
{
    public interface IGetBatchItemCount
    {
        int Get(Batch batch);
    }
}