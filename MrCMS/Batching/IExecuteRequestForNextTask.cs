using MrCMS.Batching.Entities;

namespace MrCMS.Batching
{
    public interface IExecuteRequestForNextTask
    {
        void Execute(BatchRun run);
    }
}