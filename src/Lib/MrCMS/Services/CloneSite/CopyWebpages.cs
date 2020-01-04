using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;

namespace MrCMS.Services.CloneSite
{
    [CloneSitePart(-75)]
    public class CopyWebpages : ICloneSiteParts
    {
        private readonly IGlobalRepository<Webpage> _repository;

        public CopyWebpages(IGlobalRepository<Webpage> repository)
        {
            _repository = repository;
        }

        public async Task Clone(Site @from, Site to, SiteCloneContext siteCloneContext)
        {
            var copies = GetWebpageCopies(@from, to, siteCloneContext).ToList();

            await _repository.AddRange(copies);
        }

        private IEnumerable<Webpage> GetWebpageCopies(Site @from, Site to, SiteCloneContext siteCloneContext,
            Webpage fromParent = null,
            Webpage toParent = null)
        {
            var parentId = fromParent?.Id;
            var webpages = _repository.Query().Where(webpage => webpage.Site.Id == @from.Id && webpage.ParentId == parentId).ToList();
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