using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Services.CloneSite;
using MrCMS.Web.Apps.Core.Pages;
using NHibernate;

namespace MrCMS.Web.Apps.Core.Services.CloneSites
{
    [CloneSitePart(0)]
    public class CopyRegister : ICloneSiteParts
    {
        private readonly ISession _session;

        public CopyRegister(ISession session)
        {
            _session = session;
        }

        public void Clone(Site @from, Site to)
        {
            var uniquePageService = new UniquePageService(_session,@from);
            var registerPage = uniquePageService.GetUniquePage<RegisterPage>();

            if (registerPage != null)
            {
                var copy = registerPage.GetCopyForSite(to);
                _session.Transact(session => session.Save(copy));
            }
        }
    }
}