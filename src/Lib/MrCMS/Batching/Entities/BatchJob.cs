using System.Collections.Generic;
using MrCMS.Entities;

namespace MrCMS.Batching.Entities
{
    public abstract class BatchJob : SiteEntity, IHaveJobExecutionStatus
    {
        public virtual string Data { get; set; }
        public virtual int Tries { get; set; }
        public virtual Batch Batch { get; set; }
        public virtual IList<BatchRunResult> BatchRunResults { get; set; }
        public abstract string Name { get; }
        public virtual JobExecutionStatus Status { get; set; }
    }
}