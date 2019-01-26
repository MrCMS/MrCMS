using System.Collections.Generic;
using MrCMS.Entities;

namespace MrCMS.Batching.Entities
{
    public class BatchRun : SiteEntity
    {
        public virtual Batch Batch { get; set; }
        public virtual BatchRunStatus Status { get; set; }
        public virtual IList<BatchRunResult> BatchRunResults { get; set; }
    }
}