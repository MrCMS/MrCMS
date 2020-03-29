using MrCMS.Batching.Entities;
using MrCMS.Web.Areas.Admin.Services.Batching;

namespace MrCMS.Web.Areas.Admin.Helpers
{
    public static class BatchEntityHelper
    {
        public static object ToSimpleJson(this BatchRun batchRun, BatchCompletionStatus completionStatus)
        {
            return new
            {
                guid = batchRun.Guid,
                id = batchRun.Id,
                status = batchRun.Status.ToString(),
                completionStatus
            };
        }
    }
}