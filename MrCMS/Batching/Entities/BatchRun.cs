using System;
using System.Collections.Generic;
using MrCMS.Entities;

namespace MrCMS.Batching.Entities
{
    public class BatchRun : SiteEntity
    {
        public BatchRun()
        {
            Guid = Guid.NewGuid();
        }
        public virtual Guid Guid { get; set; }
        public virtual Batch Batch { get; set; }
        public virtual BatchRunStatus Status { get; set; }
        public virtual IList<BatchRunResult> BatchRunResults { get; set; }

    }

    public enum BatchRunStatus
    {
        Pending,
        Executing,
        Paused,
        Complete
    }
}