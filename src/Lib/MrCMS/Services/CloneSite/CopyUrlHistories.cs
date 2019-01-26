using System.Linq;
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

        public override void ClonePart(Webpage @from, Webpage to, SiteCloneContext siteCloneContext)
        {
            if (!@from.Urls.Any())
                return;
            _session.Transact(session =>
            {
                foreach (var urlHistory in @from.Urls)
                {
                    var copy = urlHistory.GetCopyForSite(to.Site);
                    copy.Webpage = to;
                    to.Urls.Add(copy);
                    siteCloneContext.AddEntry(urlHistory, copy);
                    session.Save(copy);
                    session.Update(to);
                }
            });
        }
    }
}