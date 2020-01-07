using FakeItEasy;
using FluentAssertions;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Web.Apps.Admin.Tests.Stubs;
using System.Collections.Generic;
using Xunit;

namespace MrCMS.Web.Apps.Admin.Tests.Services
{
    public class WebpageWidgetAdminServiceTests
    {
        public WebpageWidgetAdminServiceTests()
        {
            _webpageWidgetAdminService = new WebpageWidgetAdminService(_webpageRepository, _widgetRepository);
            // persist current user for events
            //Context.Transact(session => session.Save(CurrentRequestData.CurrentUser));
        }

        private readonly WebpageWidgetAdminService _webpageWidgetAdminService;
        private readonly IRepository<Webpage> _webpageRepository = A.Fake<IRepository<Webpage>>();
        private readonly IRepository<Widget> _widgetRepository = A.Fake<IRepository<Widget>>();

        [Fact]
        public void WebpageWidgetAdminService_HideWidget_AddsAWidgetToTheHiddenWidgetsListIfItIsNotInTheShownList()
        {
            var stubWebpage = new StubWebpage { ShownWidgets = new HashSet<Widget>(), HiddenWidgets = new HashSet<Widget>() };
            var textWidget = new BasicMappedWidget();
            A.CallTo(() => _webpageRepository.Get(123)).Returns(stubWebpage);
            A.CallTo(() => _widgetRepository.Get(234)).Returns(textWidget);

            _webpageWidgetAdminService.Hide(123, 234);

            stubWebpage.HiddenWidgets.Should().Contain(textWidget);
        }

        [Fact]
        public void WebpageWidgetAdminService_HideWidget_DoesNothingIfTheWidgetIdIsInvalid()
        {
            var textWidget = new BasicMappedWidget();
            var stubWebpage = new StubWebpage
            {
                ShownWidgets = new HashSet<Widget> { textWidget },
                HiddenWidgets = new HashSet<Widget>()
            };
            A.CallTo(() => _webpageRepository.Get(123)).Returns(stubWebpage);
            A.CallTo(() => _widgetRepository.Get(-1)).Returns(null);

            _webpageWidgetAdminService.Hide(123, -1);

            stubWebpage.ShownWidgets.Should().Contain(textWidget);
        }

        [Fact]
        public void WebpageWidgetAdminService_HideWidget_RemovesAWidgetFromTheShownListIfItIsIncluded()
        {
            var textWidget = new BasicMappedWidget();
            A.CallTo(() => _widgetRepository.Get(234)).Returns(textWidget);
            var stubWebpage = new StubWebpage
            {
                ShownWidgets = new HashSet<Widget> { textWidget },
                HiddenWidgets = new HashSet<Widget>()
            };
            A.CallTo(() => _webpageRepository.Get(123)).Returns(stubWebpage);

            _webpageWidgetAdminService.Hide(123, 234);

            stubWebpage.ShownWidgets.Should().NotContain(textWidget);
        }


        [Fact]
        public void WebpageWidgetAdminService_ShowWidget_AddsAWidgetToTheShownWidgetsListIfItIsNotInTheHiddenList()
        {
            var stubWebpage = new StubWebpage { ShownWidgets = new HashSet<Widget>(), HiddenWidgets = new HashSet<Widget>() };
            A.CallTo(() => _webpageRepository.Get(123)).Returns(stubWebpage);
            var textWidget = new BasicMappedWidget();
            A.CallTo(() => _widgetRepository.Get(234)).Returns(textWidget);

            _webpageWidgetAdminService.Show(123, 234);

            stubWebpage.ShownWidgets.Should().Contain(textWidget);
        }

        [Fact]
        public void WebpageWidgetAdminService_ShowWidget_DoesNothingIfTheWidgetIdIsInvalid()
        {
            var textWidget = new BasicMappedWidget();

            var stubWebpage = new StubWebpage
            {
                ShownWidgets = new HashSet<Widget>(),
                HiddenWidgets = new HashSet<Widget> { textWidget }
            };
            A.CallTo(() => _webpageRepository.Get(123)).Returns(stubWebpage);
            A.CallTo(() => _widgetRepository.Get(-1)).Returns(null);


            _webpageWidgetAdminService.Show(123, -1);

            stubWebpage.HiddenWidgets.Should().Contain(textWidget);
        }

        [Fact]
        public void WebpageWidgetAdminService_ShowWidget_RemovesAWidgetFromTheHiddenListIfItIsIncluded()
        {
            var textWidget = new BasicMappedWidget();
            A.CallTo(() => _widgetRepository.Get(234)).Returns(textWidget);

            var stubWebpage = new StubWebpage
            {
                ShownWidgets = new HashSet<Widget>(),
                HiddenWidgets = new HashSet<Widget> { textWidget }
            };
            A.CallTo(() => _webpageRepository.Get(123)).Returns(stubWebpage);

            _webpageWidgetAdminService.Show(123, 234);

            stubWebpage.HiddenWidgets.Should().NotContain(textWidget);
        }
    }
}