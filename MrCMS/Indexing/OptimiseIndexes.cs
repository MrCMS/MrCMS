using MrCMS.Indexing.Management;
using MrCMS.Services;
using MrCMS.Tasks;

namespace MrCMS.Indexing
{
    public class OptimiseIndexes : SchedulableTask
    {
        private readonly IIndexService _indexService;

        public OptimiseIndexes(IIndexService indexService)
        {
            _indexService = indexService;
        }

        public override int Priority
        {
            get { return 0; }
        }

        protected override void OnExecute()
        {
            foreach (IIndexManagerBase indexManager in _indexService.GetAllIndexManagers())
            {
                indexManager.Optimise();
            }
        }
    }
}