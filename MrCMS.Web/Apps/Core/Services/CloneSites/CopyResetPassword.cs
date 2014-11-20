using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Services.CloneSite;
using MrCMS.Web.Apps.Core.Pages;
using NHibernate;

namespace MrCMS.Web.Apps.Core.Services.CloneSites
{
    [CloneSitePart(0)]
    public class CopyResetPassword : ICloneSiteParts
    {
        private readonly ISession _session;

        public CopyResetPassword(ISession session)
        {
            _session = session;
        }

        public void Clone(Site @from, Site to)
        {
            var uniquePageService = new UniquePageService(_session,@from);
            var resetPasswordPage = uniquePageService.GetUniquePage<ResetPasswordPage>();

            if (resetPasswordPage != null)
            {
                var copy = resetPasswordPage.GetCopyForSite(to);
                _session.Transact(session => session.Save(copy));
            }
        }
    }
}