using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using NHibernate;

namespace MrCMS.Services.CloneSite
{
    [CloneSitePart(-70)]
    public class UpdateWebpages : ICloneSiteParts
    {
        private readonly ICloneWebpageSiteParts _cloneWebpageSiteParts;
        private readonly ISession _session;

        public UpdateWebpages(ICloneWebpageSiteParts cloneWebpageSiteParts, ISession session)
        {
            _cloneWebpageSiteParts = cloneWebpageSiteParts;
            _session = session;
        }

        public async Task Clone(Site @from, Site to, SiteCloneContext siteCloneContext)
        {
            var webpages = await _session.QueryOver<Webpage>().Where(webpage => webpage.Site.Id == to.Id).ListAsync();
            foreach (var webpage in webpages)
            {
                var original = siteCloneContext.GetOriginal(webpage);
                if (original != null)
                    await _cloneWebpageSiteParts.Clone(original, webpage, siteCloneContext);
            }
        }
    }
}