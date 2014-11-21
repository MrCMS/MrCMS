using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Services.CloneSite
{
    public class CopyFormProperties : CloneWebpagePart<Webpage>
    {
        private readonly ISession _session;

        public CopyFormProperties(ISession session)
        {
            _session = session;
        }

        public override void ClonePart(Webpage @from, Webpage to, SiteCloneContext siteCloneContext)
        {
            if (!@from.FormProperties.Any())
                return;
            _session.Transact(session =>
            {
                foreach (var property in @from.FormProperties)
                {
                    var copy = property.GetCopyForSite(to.Site);
                    copy.Webpage = to;
                    if (to.FormProperties == null)
                        to.FormProperties = new List<FormProperty>();
                    to.FormProperties.Add(copy);
                    siteCloneContext.AddEntry(property, copy);
                    session.Save(copy);
                    session.Update(to);
                }
            });
        }
    }
}