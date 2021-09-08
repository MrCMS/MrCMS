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
    }
}