using System;
using MrCMS.Entities;

namespace MrCMS.Batching.Entities
{
    public class BatchRunResult : SiteEntity, IHaveJobExecutionStatus
    {
        public BatchRunResult()
        {
            Guid = Guid.NewGuid();
        }
        public virtual Guid Guid { get; set; }
        public virtual BatchRun BatchRun { get; set; }
        public virtual BatchJob BatchJob { get; set; }
        public virtual JobExecutionStatus Status { get; set; }
        public virtual int ExecutionOrder { get; set; }

        public virtual decimal? MillisecondsTaken { get; set; }
    }
}