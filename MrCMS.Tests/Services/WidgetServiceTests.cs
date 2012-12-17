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
        [Fact]
        public void WidgetService_GetWidget_ReturnsAWidgetWhenIdExists()
        {
            var widgetService = new WidgetService(Session);

            var textWidget = new BasicMappedWidget();
            Session.Transact(session => session.SaveOrUpdate(textWidget));

            var loadedWidget = widgetService.GetWidget<BasicMappedWidget>(textWidget.Id);

            loadedWidget.Should().BeSameAs(textWidget);
        }

        [Fact]
        public void WidgetService_GetWidget_WhenIdIsInvalidShouldReturnNull()
        {
            var widgetService = new WidgetService(Session);

            var loadedWidget = widgetService.GetWidget<BasicMappedWidget>(-1);

            loadedWidget.Should().BeNull();
        }

        [Fact]
        public void WidgetService_SaveWidget_ShouldAddWidgetToDb()
        {
            var widgetService = new WidgetService(Session);

            widgetService.SaveWidget(new BasicMappedWidget());

            Session.QueryOver<Widget>().RowCount().Should().Be(1);
        }

        [Fact]
        public void WidgetService_GetModel_CallsWidgetGetModelOfTheWidgetWithTheSessionOfTheService()
        {
            var session = A.Fake<ISession>();
            var widgetService = new WidgetService(session);

            var widget = A.Fake<Widget>();

            widgetService.GetModel(widget);

            A.CallTo(() => widget.GetModel(session)).MustHaveHappened();
        }

        [Fact]
        public void WidgetService_DeleteModelWithWidget_CallsSessionDeleteOnTheWidget()
        {
            var session = A.Fake<ISession>();
            var widgetService = new WidgetService(session);
            
            var widget = A.Fake<Widget>();

            widgetService.DeleteWidget(widget);

            A.CallTo(() => session.Delete(widget)).MustHaveHappened();
        }

        [Fact]
        public void WidgetService_DeleteModelWithId_LoadsWidgetFromSession()
        {
            var session = A.Fake<ISession>();
            var widgetService = new WidgetService(session);

            var widget = A.Fake<Widget>();

            A.CallTo(() => session.Get<Widget>(1)).Returns(widget);
            widgetService.DeleteWidget(1);

            A.CallTo(() => session.Get<Widget>(1)).MustHaveHappened();
        }

        [Fact]
        public void WidgetService_DeleteModelWithId_CallsDeleteOnTheLoadedWidget()
        {
            var session = A.Fake<ISession>();
            var widgetService = new WidgetService(session);

            var widget = A.Fake<Widget>();

            A.CallTo(() => session.Get<Widget>(1)).Returns(widget);
            widgetService.DeleteWidget(1);

            A.CallTo(() => session.Delete(widget)).MustHaveHappened();
        }
    }
}