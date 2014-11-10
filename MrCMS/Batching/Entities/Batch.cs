using System;
using System.Collections.Generic;
using MrCMS.Entities;

namespace MrCMS.Batching.Entities
{
    public class Batch : SiteEntity
    {
        public Batch()
        {
            Guid = Guid.NewGuid();
        }
        public virtual Guid Guid { get; set; }

        public virtual IList<BatchJob> BatchJobs { get; set; }
        public virtual IList<BatchRun> BatchRuns { get; set; }
    }
}