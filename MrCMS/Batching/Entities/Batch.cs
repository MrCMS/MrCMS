using System.Collections.Generic;
using MrCMS.Entities;

namespace MrCMS.Batching.Entities
{
    public class Batch : SiteEntity
    {
        public virtual IList<BatchJob> BatchJobs { get; set; }
        public virtual IList<BatchRun> BatchRuns { get; set; }
    }
}