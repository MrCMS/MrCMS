using System.Threading.Tasks;
using FluentAssertions;
using MrCMS.Entities.Widget;
using MrCMS.Services;
using MrCMS.Tests.Stubs;
using Xunit;
using MrCMS.Helpers;
using MrCMS.TestSupport;

namespace MrCMS.Tests.Services
{
    public class WidgetServiceTests : InMemoryDatabaseTest
    {
        private readonly WidgetService _widgetService;

        public WidgetServiceTests()
        {
            _widgetService = new WidgetService(Session);
        }
        [Fact]
        public async Task WidgetService_GetWidget_ReturnsAWidgetWhenIdExists()
        {

            var textWidget = new BasicMappedWidget();
            await Session.TransactAsync(session => session.SaveOrUpdateAsync(textWidget));

            var loadedWidget = await _widgetService.GetWidget<BasicMappedWidget>(textWidget.Id);

            loadedWidget.Should().BeSameAs(textWidget);
        }

        [Fact]
        public async Task WidgetService_GetWidget_WhenIdIsInvalidShouldReturnNull()
        {
            var loadedWidget = await _widgetService.GetWidget<BasicMappedWidget>(-1);

            loadedWidget.Should().BeNull();
        }

        [Fact]
        public async Task WidgetService_SaveWidget_ShouldAddWidgetToDb()
        {
            await _widgetService.SaveWidget(new BasicMappedWidget());

            (await Session.QueryOver<Widget>().RowCountAsync()).Should().Be(1);
        }

        [Fact]
        public async Task WidgetService_Delete_RemovesWidgetFromDatabase()
        {
            var widget = new BasicMappedWidget();
            await Session.TransactAsync(session => session.SaveAsync(widget));

            await _widgetService.DeleteWidget(widget);

            (await Session.QueryOver<Widget>().RowCountAsync()).Should().Be(0);
        }
    }
}