using MrCMS.Batching.Entities;

namespace MrCMS.Web.Areas.Admin.Services.Batching
{
    public interface IExecuteRequestForNextTask
    {
        void Execute(BatchRun run);
    }
}