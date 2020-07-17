using MrCMS.Batching.Entities;
using MrCMS.Web.Admin.Services.Batching;

namespace MrCMS.Web.Admin.Helpers
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