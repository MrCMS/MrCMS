using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;

namespace MrCMS.Events
{
    public class AssignLayoutAreaToLayout : OnDataAdded<LayoutArea>
    {
        private readonly IRepository<LayoutArea> _repository;
        private readonly IRepository<Layout> _layoutRepository;

        public AssignLayoutAreaToLayout(IRepository<LayoutArea> repository, IRepository<Layout> layoutRepository)
        {
            _repository = repository;
            _layoutRepository = layoutRepository;
        }

        public override async Task Execute(EntityData data)
        {
            var id = data.EntityId;

            var layoutArea = await _repository.Load(id,default, x => x.Layout);
            var layout = layoutArea.Layout;
            if (layout == null)
                return;

            layout.LayoutAreas.Add(layoutArea);
            await _layoutRepository.Update(layout);
        }
    }
}