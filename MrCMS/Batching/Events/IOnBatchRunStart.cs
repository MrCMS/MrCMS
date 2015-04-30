using MrCMS.Batching.Entities;
using MrCMS.Events;

namespace MrCMS.Batching.Events
{
    public interface IOnBatchRunStart : IEvent<BatchRunStartArgs>
    {
         
    }

    public class BatchRunStartArgs
    {
        public BatchRun BatchRun { get; set; }
    }
}