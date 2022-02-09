using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task Clone(Site @from, Site to, SiteCloneContext siteCloneContext)
        {
            var copies = await GetLayoutCopies(@from, to, siteCloneContext);

            await _session.TransactAsync(async session =>
            {
                foreach (var layout in copies)
                {
                    await session.SaveAsync(layout);
                    var layoutAreas = layout.LayoutAreas.ToList();
                    foreach (var layoutArea in layoutAreas)
                    {
                        await session.SaveAsync(layoutArea);
                        foreach (var widget in layoutArea.Widgets)
                        {
                            await session.SaveAsync(widget);
                        }
                    }
                }
            });
        }

        private async Task<IReadOnlyList<Layout>> GetLayoutCopies(Site @from, Site to,
            SiteCloneContext siteCloneContext,
            Layout fromParent = null, Layout toParent = null)
        {
            var queryOver = _session.QueryOver<Layout>().Where(layout => layout.Site.Id == @from.Id);
            queryOver = fromParent == null
                ? queryOver.Where(layout => layout.Parent == null)
                : queryOver.Where(layout => layout.Parent.Id == fromParent.Id);
            var layouts = await queryOver.ListAsync();

            var list = new List<Layout>();

            foreach (var layout in layouts)
            {
                var copy = layout.GetCopyForSite(to);
                siteCloneContext.AddEntry(layout, copy);
                copy.LayoutAreas = layout.LayoutAreas.Select(area =>
                {
                    var areaCopy = area.GetCopyForSite(to);
                    siteCloneContext.AddEntry(area, areaCopy);
                    areaCopy.Layout = copy;
                    areaCopy.Widgets = area.Widgets
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
                list.Add(copy);
                list.AddRange(await GetLayoutCopies(@from, to, siteCloneContext, layout, copy));
            }

            return list;
        }
    }
}