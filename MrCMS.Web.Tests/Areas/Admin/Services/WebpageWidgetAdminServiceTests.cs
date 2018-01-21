using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Web.Tests.Stubs;
using MrCMS.Web.Tests.TestSupport;
using MrCMS.Website;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Services
{
    public class WebpageWidgetAdminServiceTests 
    {
        public WebpageWidgetAdminServiceTests()
        {
            _webpageWidgetAdminService = new WebpageWidgetAdminService(_webpageRepository, _widgetRepository);
            // persist current user for events
            //Session.Transact(session => session.Save(CurrentRequestData.CurrentUser));
        }

        private readonly WebpageWidgetAdminService _webpageWidgetAdminService;
        private readonly IRepository<Webpage> _webpageRepository = A.Fake<IRepository<Webpage>>();
        private readonly IRepository<Widget> _widgetRepository = A.Fake<IRepository<Widget>>();

        [Fact]
        public void WebpageWidgetAdminService_HideWidget_AddsAWidgetToTheHiddenWidgetsListIfItIsNotInTheShownList()
        {
            var textPage = new StubWebpage {ShownWidgets = new HashSet<Widget>(), HiddenWidgets = new HashSet<Widget>()};
            //Session.Transact(session => session.Save(textPage));

            var textWidget = new BasicMappedWidget();
            A.CallTo(() => _widgetRepository.Get(123)).Returns(textWidget);

            //Session.Transact(session => session.Save(textWidget));

            _webpageWidgetAdminService.Hide(textPage, 123);

            textPage.HiddenWidgets.Should().Contain(textWidget);
        }

        [Fact]
        public void WebpageWidgetAdminService_HideWidget_DoesNothingIfTheWidgetIdIsInvalid()
        {
            var textWidget = new BasicMappedWidget();

            var textPage = new StubWebpage
            {
                ShownWidgets = new HashSet<Widget> {textWidget},
                HiddenWidgets = new HashSet<Widget>()
            };

            _webpageWidgetAdminService.Hide(textPage, -1);

            textPage.ShownWidgets.Should().Contain(textWidget);
        }

        [Fact]
        public void WebpageWidgetAdminService_HideWidget_RemovesAWidgetFromTheShownListIfItIsIncluded()
        {
            var textWidget = new BasicMappedWidget();
            A.CallTo(() => _widgetRepository.Get(123)).Returns(textWidget);

            var textPage = new StubWebpage
            {
                ShownWidgets = new HashSet<Widget> {textWidget},
                HiddenWidgets = new HashSet<Widget>()
            };

            _webpageWidgetAdminService.Hide(textPage, 123);

            textPage.ShownWidgets.Should().NotContain(textWidget);
        }


        [Fact]
        public void WebpageWidgetAdminService_ShowWidget_AddsAWidgetToTheShownWidgetsListIfItIsNotInTheHiddenList()
        {
            var textPage = new StubWebpage {ShownWidgets = new HashSet<Widget>(), HiddenWidgets = new HashSet<Widget>()};

            var textWidget = new BasicMappedWidget();
            A.CallTo(() => _widgetRepository.Get(123)).Returns(textWidget);

            _webpageWidgetAdminService.Show(textPage, 123);

            textPage.ShownWidgets.Should().Contain(textWidget);
        }

        [Fact]
        public void WebpageWidgetAdminService_ShowWidget_DoesNothingIfTheWidgetIdIsInvalid()
        {
            var textWidget = new BasicMappedWidget();

            var textPage = new StubWebpage
            {
                ShownWidgets = new HashSet<Widget>(),
                HiddenWidgets = new HashSet<Widget> {textWidget}
            };

            _webpageWidgetAdminService.Show(textPage, -1);

            textPage.HiddenWidgets.Should().Contain(textWidget);
        }

        [Fact]
        public void WebpageWidgetAdminService_ShowWidget_RemovesAWidgetFromTheHiddenListIfItIsIncluded()
        {
            var textWidget = new BasicMappedWidget();
            A.CallTo(() => _widgetRepository.Get(123)).Returns(textWidget);

            var textPage = new StubWebpage
            {
                ShownWidgets = new HashSet<Widget>(),
                HiddenWidgets = new HashSet<Widget> {textWidget}
            };

            _webpageWidgetAdminService.Show(textPage, 123);

            textPage.HiddenWidgets.Should().NotContain(textWidget);
        }
    }
}