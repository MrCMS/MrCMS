using MrCMS.Batching.Entities;
using MrCMS.Events;

namespace MrCMS.Batching.Events
{
    public interface IOnBatchRunPause : IEvent<BatchRunPauseArgs>
    {
         
    }

    public class BatchRunPauseArgs
    {
        public BatchRun BatchRun { get; set; }
    }
}