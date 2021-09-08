using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Services.CloneSite
{
    public class CopyUrlHistories : CloneWebpagePart<Webpage>
    {
        private readonly ISession _session;

        public CopyUrlHistories(ISession session)
        {
            _session = session;
        }

        public override async Task ClonePart(Webpage @from, Webpage to, SiteCloneContext siteCloneContext)
        {
            if (!@from.Urls.Any())
                return;
            await _session.TransactAsync(async session =>
            {
                foreach (var urlHistory in @from.Urls)
                {
                    var copy = urlHistory.GetCopyForSite(to.Site);
                    copy.Webpage = to;
                    to.Urls.Add(copy);
                    siteCloneContext.AddEntry(urlHistory, copy);
                    await session.SaveAsync(copy);
                    await session.UpdateAsync(to);
                }
            });
        }
    }
}