using System.Threading.Tasks;
using MrCMS.Batching.Entities;

namespace MrCMS.Batching.Services
{
    public interface ISynchronousBatchRunExecution
    {
        Task Execute(BatchRun run);
    }
}