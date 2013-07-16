using System.Linq;
using MrCMS.Entities;
using MrCMS.Services;

namespace MrCMS.Tasks
{
    public class UpdateIndicesTask : IndexManagementTask
    {
        public UpdateIndicesTask(SiteEntity entity)
            : base(entity)
        {
        }

        protected override void ExecuteLogic()
        {
            var definitionTypes = GetDefinitionTypes(Entity.GetType());
            foreach (var indexManagerBase in definitionTypes.Select(IndexService.GetIndexManagerBase))
                indexManagerBase.Update(Entity);
        }
    }
}