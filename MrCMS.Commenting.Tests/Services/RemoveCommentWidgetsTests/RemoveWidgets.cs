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

namespace MrCMS.Commenting.Tests.Services.RemoveCommentWidgetsTests
{
    public class RemoveWidgets : InMemoryDatabaseTest
    {
        [Fact]
        public void RemovesCommentWidgetsFromTheDesignatedArea()
        {
            BasicMappedWebpage basicMappedWebpage =
                new CommentingMappedWebpageBuilder().WithCommentsArea().WithCommentsWidget().Build();
            Session.Transact(session => session.Save(basicMappedWebpage));
            LayoutArea commentsLayoutArea = basicMappedWebpage.GetCommentsLayoutArea();
            RemoveCommentWidgets removeCommentWidgets = new RemoveCommentWidgetsBuilder().WithSession(Session).Build();

            removeCommentWidgets.RemoveWidgets(typeof (BasicMappedWebpage));

            basicMappedWebpage.Widgets.OfType<CommentingWidget>()
                              .Where(widget => widget.LayoutArea == commentsLayoutArea)
                              .Should()
                              .HaveCount(0);
        }

        [Fact]
        public void DoesNothingIfNoWidgetExists()
        {
            BasicMappedWebpage basicMappedWebpage =
                new CommentingMappedWebpageBuilder().WithCommentsArea().Build();
            Session.Transact(session => session.Save(basicMappedWebpage));
            LayoutArea commentsLayoutArea = basicMappedWebpage.GetCommentsLayoutArea();
            RemoveCommentWidgets removeCommentWidgets = new RemoveCommentWidgetsBuilder().WithSession(Session).Build();

            removeCommentWidgets.RemoveWidgets(typeof (BasicMappedWebpage));

            basicMappedWebpage.Widgets.OfType<CommentingWidget>()
                              .Where(widget => widget.LayoutArea == commentsLayoutArea)
                              .Should()
                              .HaveCount(0);
        }
    }
}