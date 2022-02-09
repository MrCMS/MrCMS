using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Batching.Entities;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Services.Batching;

namespace MrCMS.Web.Admin.Helpers
{
    public static class BatchExtensions
    {
        public static async Task<int> GetBatchItemCount(this IHtmlHelper helper, Batch batch)
        {
            return await helper.GetRequiredService<IGetBatchItemCount>().Get(batch);
        }

        public static async Task<BatchStatus> GetBatchStatus(this IHtmlHelper helper, Batch batch)
        {
            return await helper.GetRequiredService<IGetBatchStatus>().Get(batch);
        }
    }
}