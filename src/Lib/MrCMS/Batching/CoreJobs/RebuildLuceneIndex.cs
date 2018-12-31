using MrCMS.Batching.Entities;

namespace MrCMS.Batching.CoreJobs
{
    public class RebuildLuceneIndex : BatchJob
    {
        public override string Name
        {
            get { return "Rebuild Lucene Index - " + IndexName; }
        }

        public virtual string IndexName { get; set; }
    }
}