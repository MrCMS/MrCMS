using MrCMS.Batching.Entities;
using MrCMS.Web.Areas.Admin.Services.Batching;
using MrCMS.Website;

namespace MrCMS.Web.Areas.Admin.Helpers
{
    public static class BatchExtensions
    {
        public static int GetItemCount(this Batch batch)
        {
            return MrCMSApplication.Get<IGetBatchItemCount>().Get(batch);
        }

        public static BatchStatus GetStatus(this Batch batch)
        {
            return MrCMSApplication.Get<IGetBatchStatus>().Get(batch);
        }
    }
}