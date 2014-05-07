using FakeItEasy;
using MrCMS.Entities.Multisite;
using MrCMS.Web.Apps.Commenting.Services;
using NHibernate;

namespace MrCMS.Commenting.Tests.Services.CommentAdminServiceTests
{
    public class CommentAdminServiceBuilder
    {
        private ISession _session = A.Fake<ISession>();
        private Site _site = new Site();

        public CommentAdminService Build()
        {
            return new CommentAdminService(_session, _site);
        }

        public CommentAdminServiceBuilder WithSession(ISession session)
        {
            _session = session;
            return this;
        }
        public CommentAdminServiceBuilder WithSite(Site site)
        {
            _site = site;
            return this;
        }
    }
}