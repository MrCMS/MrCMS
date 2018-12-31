using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Services.CloneSite
{
    [CloneSitePart(-75)]
    public class CopyWebpages : ICloneSiteParts
    {
        private readonly ISession _session;

        public CopyWebpages(ISession session)
        {
            _session = session;
        }

        public void Clone(Site @from, Site to, SiteCloneContext siteCloneContext)
        {
            IEnumerable<Webpage> copies = GetWebpageCopies(@from, to, siteCloneContext).ToList();

            _session.Transact(session => copies.ForEach(webpage => session.Save(webpage)));
        }

        private IEnumerable<Webpage> GetWebpageCopies(Site @from, Site to, SiteCloneContext siteCloneContext,
            Webpage fromParent = null,
            Webpage toParent = null)
        {
            IQueryOver<Webpage, Webpage> queryOver =
                _session.QueryOver<Webpage>().Where(webpage => webpage.Site.Id == @from.Id);
            queryOver = fromParent == null
                ? queryOver.Where(webpage => webpage.Parent == null)
                : queryOver.Where(webpage => webpage.Parent.Id == fromParent.Id);
            IList<Webpage> webpages = queryOver.List();
            foreach (Webpage webpage in webpages)
            {
                Webpage copy = webpage.GetCopyForSite(to);
                siteCloneContext.AddEntry(webpage, copy);
                copy.Parent = toParent;
                yield return copy;
                foreach (Webpage child in GetWebpageCopies(@from, to, siteCloneContext, webpage, copy))
                {
                    yield return child;
                }
            }
        }
    }
}