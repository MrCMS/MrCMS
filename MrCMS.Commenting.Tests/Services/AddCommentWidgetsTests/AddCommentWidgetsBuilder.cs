using FakeItEasy;
using MrCMS.Web.Apps.Commenting.Services;
using NHibernate;

namespace MrCMS.Commenting.Tests.Services.AddCommentWidgetsTests
{
    public class AddCommentWidgetsBuilder
    {
        private ISession _session = A.Fake<ISession>();

        public AddCommentWidgetsBuilder WithSession(ISession session)
        {
            _session = session;
            return this;
        }

        public AddCommentWidgets Build()
        {
            return new AddCommentWidgets(_session);
        }
    }
}