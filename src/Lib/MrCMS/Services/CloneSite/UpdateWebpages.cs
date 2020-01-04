using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;

namespace MrCMS.Services.CloneSite
{
    [CloneSitePart(-70)]
    public class UpdateWebpages : ICloneSiteParts
    {
        private readonly ICloneWebpageSiteParts _cloneWebpageSiteParts;
        private readonly IGlobalRepository<Webpage> _repository;

        public UpdateWebpages(ICloneWebpageSiteParts cloneWebpageSiteParts, IGlobalRepository<Webpage> repository)
        {
            _cloneWebpageSiteParts = cloneWebpageSiteParts;
            _repository = repository;
        }

        public async Task Clone(Site @from, Site to, SiteCloneContext siteCloneContext)
        {
            var webpages = _repository.Query().Where(webpage => webpage.Site.Id == to.Id).ToList();
            foreach (var webpage in webpages)
            {
                var original = siteCloneContext.GetOriginal(webpage);
                if (original != null)
                    await _cloneWebpageSiteParts.Clone(original, webpage, siteCloneContext);
            }
        }
    }
}