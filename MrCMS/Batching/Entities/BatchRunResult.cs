using MrCMS.Entities;

namespace MrCMS.Batching.Entities
{
    public class BatchRunResult : SiteEntity
    {
        public virtual BatchJob BatchJob { get; set; }
        public virtual JobExecutionStatus Status { get; set; }
        public virtual int ExecutionOrder { get; set; }
    }
}