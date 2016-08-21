using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Widget;
using MrCMS.Services;
using MrCMS.Tests.Stubs;
using NHibernate;
using Xunit;
using MrCMS.Helpers;
using MrCMS.Tests.TestSupport;

namespace MrCMS.Tests.Services
{
    public class WidgetServiceTests 
    {
        private readonly WidgetService _widgetService;
        private InMemoryRepository<Widget> _inMemoryRepository = new InMemoryRepository<Widget>();

        public WidgetServiceTests()
        {
            _widgetService = new WidgetService(_inMemoryRepository);
        }
        [Fact]
        public void WidgetService_GetWidget_ReturnsAWidgetWhenIdExists()
        {

            var textWidget = new BasicMappedWidget();
            _inMemoryRepository.Add(textWidget);

            var loadedWidget = _widgetService.GetWidget<BasicMappedWidget>(textWidget.Id);

            loadedWidget.Should().BeSameAs(textWidget);
        }

        [Fact]
        public void WidgetService_GetWidget_WhenIdIsInvalidShouldReturnNull()
        {
            var loadedWidget = _widgetService.GetWidget<BasicMappedWidget>(-1);

            loadedWidget.Should().BeNull();
        }

        [Fact]
        public void WidgetService_AddWidget_ShouldAddWidgetToDb()
        {
            _widgetService.AddWidget(new BasicMappedWidget());

            _inMemoryRepository.Query().Count().Should().Be(1);
        }

        [Fact]
        public void WidgetService_Delete_RemovesWidgetFromDatabase()
        {
            var widget = new BasicMappedWidget();
            _inMemoryRepository.Add(widget);

            _widgetService.DeleteWidget(widget);

            _inMemoryRepository.Query().Count().Should().Be(0);
        }
    }
}