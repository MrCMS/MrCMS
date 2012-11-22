using FluentAssertions;
using MrCMS.Entities.Widget;
using MrCMS.Services;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.Tests.Services
{
    public class WidgetServiceTests : InMemoryDatabaseTest
    {
        [Fact]
        public void WidgetService_GetWidget_ReturnsAWidgetWhenIdExists()
        {
            var widgetService = new WidgetService(Session);

            var textWidget = new TextWidget();
            Session.Transact(session => session.SaveOrUpdate(textWidget));

            var loadedWidget = widgetService.GetWidget<TextWidget>(textWidget.Id);

            loadedWidget.Should().BeSameAs(textWidget);
        }

        [Fact]
        public void WidgetService_GetWidget_WhenIdIsInvalidShouldReturnNull()
        {
            var widgetService = new WidgetService(Session);

            var loadedWidget = widgetService.GetWidget<TextWidget>(-1);

            loadedWidget.Should().BeNull();
        }

        [Fact]
        public void WidgetService_SaveWidget_ShouldAddWidgetToDb()
        {
            var widgetService = new WidgetService(Session);

            widgetService.SaveWidget(new TextWidget() {Text = "text"});

            Session.QueryOver<Widget>().RowCount().Should().Be(1);
        }
    }
}