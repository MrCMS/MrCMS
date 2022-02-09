using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Services.CloneSite
{
    public class SetPageTemplate : CloneWebpagePart<Webpage>
    {
        private readonly ISession _session;

        public SetPageTemplate(ISession session)
        {
            _session = session;
        }

        public override async Task ClonePart(Webpage @from, Webpage to, SiteCloneContext siteCloneContext)
        {
            if (@from.PageTemplate == null) 
                return;
            to.PageTemplate = siteCloneContext.FindNew<PageTemplate>(@from.PageTemplate.Id);
            await _session.TransactAsync(session => session.UpdateAsync(to));
        }
    }
}