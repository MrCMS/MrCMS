using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.CloneSite
{
    public class CopyUrlHistories : CloneWebpagePart<Webpage>
    {
        private readonly IGlobalRepository<UrlHistory> _repository;

        public CopyUrlHistories(IGlobalRepository<UrlHistory> repository)
        {
            _repository = repository;
        }

        public override async Task ClonePart(Webpage @from, Webpage to, SiteCloneContext siteCloneContext)
        {
            if (!@from.Urls.Any())
                return;
            await _repository.Transact(async (repo, ct) =>
             {
                 foreach (var urlHistory in @from.Urls)
                 {
                     var copy = urlHistory.GetCopyForSite(to.Site);
                     copy.Webpage = to;
                     to.Urls.Add(copy);
                     siteCloneContext.AddEntry(urlHistory, copy);
                     await _repository.Add(copy, ct);
                 }
             });
        }
    }
}