using MrCMS.Batching.Entities;

namespace MrCMS.Batching.CoreJobs
{
    public class RebuildUniversalSearchIndex : BatchJob
    {
        public override string Name
        {
            get { return "Rebuild Universal Index"; }
        }
    }
}