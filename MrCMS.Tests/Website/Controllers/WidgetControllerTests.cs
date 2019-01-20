using System;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Services;
using MrCMS.Tests.Stubs;
using MrCMS.Tests.Website.Controllers.Builders;
using MrCMS.Website;
using Xunit;

namespace MrCMS.Tests.Website.Controllers
{
    public class WidgetControllerTests
    {
        [Fact]
        public void Show_ReturnsTheResultOfTheUIServiceGetContentCall()
        {
            var widgetUIService = A.Fake<IWidgetUIService>();
            var widgetController = new WidgetControllerBuilder().WithService(widgetUIService).Build();
            var widget = new BasicMappedWidget();
            var expectedResult = new ContentResult();
            A.CallTo(() => widgetUIService.GetContent(widgetController, widget, A<Func<IHtmlHelper,MvcHtmlString>>._)).Returns(expectedResult);

            var result = widgetController.Show(widget);

            result.Should().Be(expectedResult);
        }

        [Fact]
        public void Internal_ReturnsAPartialViewWithTheWidgetModel()
        {
            var widgetUIService = A.Fake<IWidgetUIService>();
            var widgetController = new WidgetControllerBuilder().WithService(widgetUIService).Build();
            var widget = new BasicMappedWidget();
            var widgetModel = new object();
            A.CallTo(() => widgetUIService.GetModel(widget)).Returns(widgetModel);

            var result = widgetController.Internal(widget);

            result.Should().BeOfType<PartialViewResult>();
            result.Model.Should().Be(widgetModel);
        }

        [Fact]
        public void Internal_IfTheWidgetHasNoCustomLayoutTheViewNameShouldBeTheTypeNameOfTheWidget()
        {
            var widgetUIService = A.Fake<IWidgetUIService>();
            var widgetController = new WidgetControllerBuilder().WithService(widgetUIService).Build();
            var widget = new BasicMappedWidget();

            var result = widgetController.Internal(widget);

            result.ViewName.Should().Be(typeof (BasicMappedWidget).Name);
        }

        [Fact]
        public void Internal_IfTheWidgetHasACustomLayoutUseThatAsTheViewName()
        {
            var widgetUIService = A.Fake<IWidgetUIService>();
            var widgetController = new WidgetControllerBuilder().WithService(widgetUIService).Build();
            var widget = new BasicMappedWidget{CustomLayout = "custom view"};

            var result = widgetController.Internal(widget);

            result.ViewName.Should().Be("custom view");
        }

        [Fact]
        public void Internal_SetsAppDataTokenForWidget()
        {
            var widgetUIService = A.Fake<IWidgetUIService>();
            var widgetController = new WidgetControllerBuilder().WithService(widgetUIService).Build();
            var widget = new BasicMappedWidget();
                
            widgetController.Internal(widget);

            A.CallTo(() => widgetUIService.SetAppDataToken(widgetController.RouteData, widget)).MustHaveHappened();
        }
    }
}