using FakeItEasy;
using MrCMS.Web.Apps.Commenting.Services;
using NHibernate;

namespace MrCMS.Commenting.Tests.Services.RemoveCommentWidgetsTests
{
    public class RemoveCommentWidgetsBuilder
    {
        private ISession _session = A.Fake<ISession>();

        public RemoveCommentWidgetsBuilder WithSession(ISession session)
        {
            _session = session;
            return this;
        }
         public RemoveCommentWidgets Build()
         {
             return new RemoveCommentWidgets(_session);
         }
    }
}