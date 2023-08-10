using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Widget;
using NHibernate.Linq;

namespace MrCMS.Website
{
    public class WidgetLoader : IWidgetLoader
    {
        private readonly IRepository<Widget> _widgetRepository;

        public WidgetLoader(IRepository<Widget> widgetRepository)
        {
            _widgetRepository = widgetRepository;
        }

        public async Task<IList<Widget>> GetWidgets(LayoutArea area)
        {
            return await _widgetRepository.Query()
                .Where(x => x.LayoutArea != null && x.LayoutArea.Id == area.Id)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();
        }

        public async Task<IReadOnlyDictionary<LayoutArea, IList<Widget>>> GetWidgets(
            IEnumerable<LayoutArea> layoutAreas)
        {
            var layoutAreaIds = layoutAreas.Select(x => x.Id).ToList();

            var widgets = await _widgetRepository.Query()
                .Where(x => x.LayoutArea != null && layoutAreaIds.Contains(x.LayoutArea.Id))
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();

            // create a dictionary of layout area id to widgets, using all layout areas from the parameter
            // they should already be in the correct order from the query above
            var dictionary = layoutAreas.ToDictionary(x => x, x =>
                widgets.Where(w => w.LayoutArea.Id == x.Id).ToList() as IList<Widget>);

            return dictionary;
        }
    }
}
