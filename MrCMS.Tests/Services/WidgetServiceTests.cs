using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Widget;
using MrCMS.Services;
using MrCMS.Tests.Stubs;
using NHibernate;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.Tests.Services
{
    public class WidgetServiceTests : InMemoryDatabaseTest
    {
        private WidgetService _widgetService;

        public WidgetServiceTests()
        {
            _widgetService = new WidgetService(Session);
        }
        [Fact]
        public void WidgetService_GetWidget_ReturnsAWidgetWhenIdExists()
        {

            var textWidget = new BasicMappedWidget();
            Session.Transact(session => session.SaveOrUpdate(textWidget));

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
        public void WidgetService_SaveWidget_ShouldAddWidgetToDb()
        {
            _widgetService.SaveWidget(new BasicMappedWidget());

            Session.QueryOver<Widget>().RowCount().Should().Be(1);
        }

        [Fact]
        public void WidgetService_GetModel_CallsWidgetGetModelOfTheWidgetWithTheSessionOfTheService()
        {
            var widget = A.Fake<Widget>();

            _widgetService.GetModel(widget);

            A.CallTo(() => widget.GetModel(Session)).MustHaveHappened();
        }

        [Fact]
        public void WidgetService_Delete_RemovesWidgetFromDatabase()
        {
            var widget = new BasicMappedWidget();
            Session.Transact(session => session.Save(widget));

            _widgetService.DeleteWidget(widget);

            Session.QueryOver<Widget>().RowCount().Should().Be(0);
        }

        [Fact]
        public void WidgetService_Delete_CallsOnDeletingOnTheWidget()
        {
            var widget = A.Fake<Widget>();
            var session = A.Fake<ISession>();
            var widgetService = new WidgetService(session);

            widgetService.DeleteWidget(widget);

            A.CallTo(() => widget.OnDeleting(session)).MustHaveHappened();
        }
    }
}