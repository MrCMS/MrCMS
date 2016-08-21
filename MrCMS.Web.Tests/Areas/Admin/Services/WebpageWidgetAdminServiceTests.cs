using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Web.Tests.Stubs;
using MrCMS.Web.Tests.TestSupport;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Services
{
    public class WebpageWidgetAdminServiceTests 
    {
        public WebpageWidgetAdminServiceTests()
        {
            _webpageWidgetAdminService = new WebpageWidgetAdminService(_webpageRepository, _widgetRepository);
        }

        private readonly WebpageWidgetAdminService _webpageWidgetAdminService;
        private readonly InMemoryRepository<Webpage> _webpageRepository = new InMemoryRepository<Webpage>();
        private readonly InMemoryRepository<Widget> _widgetRepository = new InMemoryRepository<Widget>();

        [Fact]
        public void WebpageWidgetAdminService_HideWidget_AddsAWidgetToTheHiddenWidgetsListIfItIsNotInTheShownList()
        {
            var textPage = new StubWebpage {ShownWidgets = new HashSet<Widget>(), HiddenWidgets = new HashSet<Widget>()};
            _webpageRepository.Add(textPage);

            var textWidget = new BasicMappedWidget();
            _widgetRepository.Add(textWidget);

            _webpageWidgetAdminService.Hide(textPage, textWidget.Id);

            textPage.HiddenWidgets.Should().Contain(textWidget);
        }

        [Fact]
        public void WebpageWidgetAdminService_HideWidget_DoesNothingIfTheWidgetIdIsInvalid()
        {
            var textWidget = new BasicMappedWidget();
            _widgetRepository.Add(textWidget);

            var textPage = new StubWebpage
            {
                ShownWidgets = new HashSet<Widget> {textWidget},
                HiddenWidgets = new HashSet<Widget>()
            };
            _webpageRepository.Add(textPage);

            _webpageWidgetAdminService.Hide(textPage, -1);

            textPage.ShownWidgets.Should().Contain(textWidget);
        }

        [Fact]
        public void WebpageWidgetAdminService_HideWidget_RemovesAWidgetFromTheShownListIfItIsIncluded()
        {
            var textWidget = new BasicMappedWidget();
            _widgetRepository.Add(textWidget);

            var textPage = new StubWebpage
            {
                ShownWidgets = new HashSet<Widget> {textWidget},
                HiddenWidgets = new HashSet<Widget>()
            };
            _webpageRepository.Add(textPage);

            _webpageWidgetAdminService.Hide(textPage, textWidget.Id);

            textPage.ShownWidgets.Should().NotContain(textWidget);
        }


        [Fact]
        public void WebpageWidgetAdminService_ShowWidget_AddsAWidgetToTheShownWidgetsListIfItIsNotInTheHiddenList()
        {
            var textPage = new StubWebpage {ShownWidgets = new HashSet<Widget>(), HiddenWidgets = new HashSet<Widget>()};
            _webpageRepository.Add(textPage);

            var textWidget = new BasicMappedWidget();
            _widgetRepository.Add(textWidget);

            _webpageWidgetAdminService.Show(textPage, textWidget.Id);

            textPage.ShownWidgets.Should().Contain(textWidget);
        }

        [Fact]
        public void WebpageWidgetAdminService_ShowWidget_DoesNothingIfTheWidgetIdIsInvalid()
        {
            var textWidget = new BasicMappedWidget();
            _widgetRepository.Add(textWidget);

            var textPage = new StubWebpage
            {
                ShownWidgets = new HashSet<Widget>(),
                HiddenWidgets = new HashSet<Widget> {textWidget}
            };
            _webpageRepository.Add(textPage);

            _webpageWidgetAdminService.Show(textPage, -1);

            textPage.HiddenWidgets.Should().Contain(textWidget);
        }

        [Fact]
        public void WebpageWidgetAdminService_ShowWidget_RemovesAWidgetFromTheHiddenListIfItIsIncluded()
        {
            var textWidget = new BasicMappedWidget();
            _widgetRepository.Add(textWidget);

            var textPage = new StubWebpage
            {
                ShownWidgets = new HashSet<Widget>(),
                HiddenWidgets = new HashSet<Widget> {textWidget}
            };
            _webpageRepository.Add(textPage);

            _webpageWidgetAdminService.Show(textPage, textWidget.Id);

            textPage.HiddenWidgets.Should().NotContain(textWidget);
        }
    }
}