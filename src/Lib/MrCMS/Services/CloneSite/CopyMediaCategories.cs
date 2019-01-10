using System.Collections.Generic;
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

        public void Clone(Site @from, Site to, SiteCloneContext siteCloneContext)
        {
            IEnumerable<MediaCategory> copies = GetMediaCategoryCopies(@from, to, siteCloneContext);

            _session.Transact(session => copies.ForEach(category => session.Save(category)));
        }

        private IEnumerable<MediaCategory> GetMediaCategoryCopies(Site @from, Site to, SiteCloneContext siteCloneContext, MediaCategory fromParent = null, MediaCategory toParent = null)
        {
            IQueryOver<MediaCategory, MediaCategory> queryOver =
                _session.QueryOver<MediaCategory>().Where(layout => layout.Site.Id == @from.Id);
            queryOver = fromParent == null
                ? queryOver.Where(layout => layout.Parent == null)
                : queryOver.Where(layout => layout.Parent.Id == fromParent.Id);
            IList<MediaCategory> categories = queryOver.List();
            foreach (MediaCategory category in categories)
            {
                MediaCategory copy = category.GetCopyForSite(to);
                siteCloneContext.AddEntry(category, copy);
                copy.Parent = toParent;
                yield return copy;
                foreach (MediaCategory child in GetMediaCategoryCopies(@from, to, siteCloneContext, fromParent: category, toParent: copy))
                {
                    yield return child;
                }
            }
        }
    }
}