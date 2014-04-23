using System.Linq;
using FluentAssertions;
using MrCMS.Commenting.Tests.Stubs;
using MrCMS.Commenting.Tests.Support;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Commenting.Extensions;
using MrCMS.Web.Apps.Commenting.Services;
using MrCMS.Web.Apps.Commenting.Widgets;
using Xunit;

namespace MrCMS.Commenting.Tests.Services.AddCommentWidgetsTests
{
    public class AddWidgets : InMemoryDatabaseTest
    {
        [Fact]
        public void IfSavedPageHasCommentsAreaAvailableAddsCommentWidgetToIt()
        {
            BasicMappedWebpage basicMappedWebpage = new CommentingMappedWebpageBuilder().WithCommentsArea().Build();
            Session.Transact(session => session.Save(basicMappedWebpage));
            LayoutArea commentsLayoutArea = basicMappedWebpage.GetCommentsLayoutArea();
            AddCommentWidgets addCommentWidgets = new AddCommentWidgetsBuilder().WithSession(Session).Build();

            addCommentWidgets.AddWidgets(typeof (BasicMappedWebpage));

            basicMappedWebpage.Widgets.OfType<CommentingWidget>()
                              .Where(widget => widget.LayoutArea == commentsLayoutArea)
                              .Should()
                              .HaveCount(1);
        }

        [Fact]
        public void IfSavedPageHasCommentsAreaAvailableAndAlreadyHasCommentsAddedDoNotAddAnother()
        {
            BasicMappedWebpage basicMappedWebpage =
                new CommentingMappedWebpageBuilder().WithCommentsArea().WithCommentsWidget().Build();
            Session.Transact(session => session.Save(basicMappedWebpage));
            LayoutArea commentsLayoutArea = basicMappedWebpage.GetCommentsLayoutArea();
            AddCommentWidgets addCommentWidgets = new AddCommentWidgetsBuilder().WithSession(Session).Build();

            addCommentWidgets.AddWidgets(typeof (BasicMappedWebpage));

            basicMappedWebpage.Widgets.OfType<CommentingWidget>()
                              .Where(widget => widget.LayoutArea == commentsLayoutArea)
                              .Should()
                              .HaveCount(1);
        }
    }
}