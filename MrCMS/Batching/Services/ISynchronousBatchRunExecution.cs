using MrCMS.Batching.Entities;

namespace MrCMS.Batching.Services
{
    public interface ISynchronousBatchRunExecution
    {
        void Execute(BatchRun run);
    }
}