using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;

namespace MrCMS.Services.CloneSite
{
    [CloneSitePart(-90)]
    public class CopyLayouts : ICloneSiteParts
    {
        private readonly IGlobalRepository<Layout> _layoutRepository;
        private readonly IGlobalRepository<LayoutArea> _layoutAreaRepository;
        private readonly IGlobalRepository<Widget> _widgetRepository;

        public CopyLayouts(IGlobalRepository<Layout> layoutRepository,
            IGlobalRepository<LayoutArea> layoutAreaRepository,
            IGlobalRepository<Widget> widgetRepository
            )
        {
            _layoutRepository = layoutRepository;
            _layoutAreaRepository = layoutAreaRepository;
            _widgetRepository = widgetRepository;
        }

        public async Task Clone(Site @from, Site to, SiteCloneContext siteCloneContext)
        {
            var copies = GetLayoutCopies(@from, to, siteCloneContext);

            await _layoutRepository.Transact(async (repo, ct) =>
            {
                foreach (var layout in copies)
                {
                    await repo.Add(layout,ct);
                    var layoutAreas = layout.LayoutAreas.ToList();
                    foreach (var layoutArea in layoutAreas)
                    {
                        await _layoutAreaRepository.Add(layoutArea,ct);
                        foreach (var widget in layoutArea.Widgets)
                        {
                            await _widgetRepository.Add(widget, ct);
                        }
                    }
                }

            });
        }
        private IEnumerable<Layout> GetLayoutCopies(Site @from, Site to, SiteCloneContext siteCloneContext, Layout fromParent = null, Layout toParent = null)
        {
            var queryOver = _layoutRepository.Query().Where(layout => layout.Site.Id == @from.Id);
            queryOver = fromParent == null
                ? queryOver.Where(layout => layout.Parent == null)
                : queryOver.Where(layout => layout.Parent.Id == fromParent.Id);
            var layouts = queryOver.ToList();

            foreach (var layout in layouts)
            {
                var copy = layout.GetCopyForSite(to);
                siteCloneContext.AddEntry(layout, copy);
                copy.LayoutAreas = _layoutAreaRepository.Query().Where(x => x.LayoutId == layout.Id).ToList().Select(area =>
                 {
                     var areaCopy = area.GetCopyForSite(to);
                     siteCloneContext.AddEntry(area, areaCopy);
                     areaCopy.Layout = copy;
                     areaCopy.Widgets = _widgetRepository.Query().Where(x => x.LayoutAreaId == area.Id).ToList()
                         .Where(widget => widget.Webpage == null)
                         .Select(widget =>
                         {
                             var widgetCopy = widget.GetCopyForSite(to);
                             siteCloneContext.AddEntry(widget, widgetCopy);
                             widgetCopy.LayoutArea = areaCopy;
                             return widgetCopy;
                         })
                         .ToList();
                     return areaCopy;
                 }).ToList();
                copy.Parent = toParent;
                yield return copy;
                foreach (var child in GetLayoutCopies(@from, to, siteCloneContext, layout, copy))
                {
                    yield return child;
                }
            }
        }
    }
}