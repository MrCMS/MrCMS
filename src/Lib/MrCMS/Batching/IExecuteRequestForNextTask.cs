using System.Threading.Tasks;
using MrCMS.Batching.Entities;

namespace MrCMS.Batching
{
    public interface IExecuteRequestForNextTask
    {
        Task Execute(BatchRun run);
    }
}