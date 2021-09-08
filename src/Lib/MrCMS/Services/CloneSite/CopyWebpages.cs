using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task Clone(Site @from, Site to, SiteCloneContext siteCloneContext)
        {
            var copies = await GetWebpageCopies(@from, to, siteCloneContext);

            await _session.TransactAsync(async session =>
            {
                foreach (var webpage in copies)
                {
                    await session.SaveAsync(webpage);
                }

                // return copies.ForEach(webpage => session.Save(webpage));
            });
        }

        private async Task<IReadOnlyList<Webpage>> GetWebpageCopies(Site @from, Site to,
            SiteCloneContext siteCloneContext,
            Webpage fromParent = null,
            Webpage toParent = null)
        {
            IQueryOver<Webpage, Webpage> queryOver =
                _session.QueryOver<Webpage>().Where(webpage => webpage.Site.Id == @from.Id);
            queryOver = fromParent == null
                ? queryOver.Where(webpage => webpage.Parent == null)
                : queryOver.Where(webpage => webpage.Parent.Id == fromParent.Id);
            IList<Webpage> webpages = await queryOver.ListAsync();
            var list = new List<Webpage>();
            foreach (Webpage webpage in webpages)
            {
                Webpage copy = webpage.GetCopyForSite(to);
                siteCloneContext.AddEntry(webpage, copy);
                copy.Parent = toParent;
                list.Add(copy);
                foreach (Webpage child in await GetWebpageCopies(@from, to, siteCloneContext, webpage, copy))
                {
                    list.Add(child);
                }
            }

            return list;
        }
    }
}