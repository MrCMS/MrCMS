using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Services.CloneSite
{
    [CloneSitePart(-90)]
    public class CopyLayouts : ICloneSiteParts
    {
        private readonly ISession _session;

        public CopyLayouts(ISession session)
        {
            _session = session;
        }

        public void Clone(Site @from, Site to)
        {
            var copies = GetLayoutCopies(@from, to);

            _session.Transact(session => copies.ForEach(layout =>
            {
                session.Save(layout);
                var layoutAreas = layout.LayoutAreas.ToList();
                foreach (var layoutArea in layoutAreas)
                {
                    session.Save(layoutArea);
                    layoutArea.Widgets.ForEach(widget => session.Save(widget));
                }
            }));
        }
        private IEnumerable<Layout> GetLayoutCopies(Site @from, Site to, Layout fromParent = null, Layout toParent = null)
        {
            var queryOver = _session.QueryOver<Layout>().Where(layout => layout.Site.Id == @from.Id);
            queryOver = fromParent == null
                ? queryOver.Where(layout => layout.Parent == null)
                : queryOver.Where(layout => layout.Parent.Id == fromParent.Id);
            var layouts = queryOver.List();

            foreach (var layout in layouts)
            {
                var copy = layout.GetCopyForSite(to);
                copy.LayoutAreas = layout.LayoutAreas.Select(area =>
                {
                    var areaCopy = CloneSiteExtensions.GetCopyForSite<LayoutArea>(area, to);
                    areaCopy.Layout = copy;
                    areaCopy.Widgets = area.Widgets
                        .Where(widget => widget.Webpage == null)
                        .Select(widget =>
                        {
                            var widgetCopy = widget.GetCopyForSite(to);
                            widgetCopy.LayoutArea = areaCopy;
                            return widgetCopy;
                        })
                        .ToList();
                    return areaCopy;
                }).ToList();
                copy.Parent = toParent;
                yield return copy;
                foreach (var child in GetLayoutCopies(@from, to, layout, copy))
                {
                    yield return child;
                }
            }
        }
    }
}