using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using X.PagedList;

namespace MrCMS.Website
{
    public class LayoutAreaLoader : ILayoutAreaLoader
    {
        private readonly IRepository<Layout> _layoutRepository;
        private readonly IRepository<LayoutArea> _layoutAreaRepository;

        public LayoutAreaLoader(IRepository<Layout> layoutRepository, IRepository<LayoutArea> layoutAreaRepository)
        {
            _layoutRepository = layoutRepository;
            _layoutAreaRepository = layoutAreaRepository;
        }

        public async Task<IEnumerable<LayoutArea>> GetLayoutAreas(int id)
        {
            return await GetLayoutAreas(await _layoutRepository.Load(id));
        }

        public async Task<IEnumerable<LayoutArea>> GetLayoutAreas(Layout layout)
        {
            Layout current = layout;
            var layoutAreas = new List<LayoutArea>();
            while (current != null)
            {
                layoutAreas.AddRange(await _layoutAreaRepository.Query().Where(x=>x.LayoutId == current.Id).ToListAsync());
                current = current.ParentId.HasValue ? await _layoutRepository.Load(current.ParentId.Value) : null;
            }

            return layoutAreas;
        }
    }
}