using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Services.CloneSite;
using MrCMS.Web.Apps.Core.Pages;
using NHibernate;

namespace MrCMS.Web.Apps.Core.Services.CloneSites
{
    [CloneSitePart(0)]
    public class CopyLogin : ICloneSiteParts
    {
        private readonly ISession _session;

        public CopyLogin(ISession session)
        {
            _session = session;
        }

        public void Clone(Site @from, Site to)
        {
            var uniquePageService = new UniquePageService(_session, @from);
            var login = uniquePageService.GetUniquePage<LoginPage>();

            if (login != null)
            {
                var loginCopy = login.GetCopyForSite(to);
                _session.Transact(session => session.Save(loginCopy));
            }
        }
    }
}