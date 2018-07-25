using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Widget;
using NHibernate;

namespace MrCMS.Website
{
    public class GetWidgetsForAreas : IGetWidgetsForAreas
    {
        private readonly ISession _session;

        public GetWidgetsForAreas(ISession session)
        {
            _session = session;
        }

        public IDictionary<LayoutArea, IList<Widget>> GetWidgets(IEnumerable<LayoutArea> layoutAreas)
        {
            var layoutAreaIds = layoutAreas.Select(x => x.Id).ToList();

            var widgets = _session.Query<Widget>()
                .Where(x => layoutAreaIds.Contains(x.LayoutArea.Id))
                .ToList();

            return layoutAreas.ToDictionary(area => area,
                area => (IList<Widget>)widgets.Where(x => x.LayoutArea.Id == area.Id).ToList());
        }
    }
}