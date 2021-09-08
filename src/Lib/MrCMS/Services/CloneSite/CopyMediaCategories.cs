using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Services.CloneSite
{
    [CloneSitePart(-80)]
    public class CopyMediaCategories : ICloneSiteParts
    {
        private readonly ISession _session;

        public CopyMediaCategories(ISession session)
        {
            _session = session;
        }

        public async Task Clone(Site @from, Site to, SiteCloneContext siteCloneContext)
        {
            var copies = await GetMediaCategoryCopies(@from, to, siteCloneContext);

            await _session.TransactAsync(async session =>
            {
                foreach (var copy in copies)
                {
                    await session.SaveAsync(copy);
                }
            });
        }

        private async Task<IReadOnlyList<MediaCategory>> GetMediaCategoryCopies(Site @from, Site to,
            SiteCloneContext siteCloneContext, MediaCategory fromParent = null, MediaCategory toParent = null)
        {
            IQueryOver<MediaCategory, MediaCategory> queryOver =
                _session.QueryOver<MediaCategory>().Where(layout => layout.Site.Id == @from.Id);
            queryOver = fromParent == null
                ? queryOver.Where(layout => layout.Parent == null)
                : queryOver.Where(layout => layout.Parent.Id == fromParent.Id);
            IList<MediaCategory> categories = await queryOver.ListAsync();
            var list = new List<MediaCategory>();
            foreach (MediaCategory category in categories)
            {
                MediaCategory copy = category.GetCopyForSite(to);
                siteCloneContext.AddEntry(category, copy);
                copy.Parent = toParent;
                list.Add(copy);
                foreach (MediaCategory child in await GetMediaCategoryCopies(@from, to, siteCloneContext,
                    fromParent: category, toParent: copy))
                {
                    list.Add(child);
                }
            }

            return list;
        }
    }
}