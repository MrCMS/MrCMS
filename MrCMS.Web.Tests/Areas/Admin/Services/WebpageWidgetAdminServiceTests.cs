using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using Iesi.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Web.Tests.Stubs;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Services
{
    public class WebpageWidgetAdminServiceTests : InMemoryDatabaseTest
    {
        private readonly IDocumentService _documentService;
        private readonly WebpageWidgetAdminService _webpageWidgetAdminService;

        public WebpageWidgetAdminServiceTests()
        {
            _documentService = A.Fake<IDocumentService>();
            _webpageWidgetAdminService = new WebpageWidgetAdminService(_documentService, Session);
        }

        [Fact]
        public void WebpageWidgetAdminService_HideWidget_AddsAWidgetToTheHiddenWidgetsListIfItIsNotInTheShownList()
        {
            var textPage = new StubWebpage { ShownWidgets = new HashSet<Widget>(), HiddenWidgets = new HashSet<Widget>() };
            Session.Transact(session => session.Save(textPage));

            var textWidget = new BasicMappedWidget();
            Session.Transact(session => session.Save(textWidget));

            _webpageWidgetAdminService.Hide(textPage, textWidget.Id);

            textPage.HiddenWidgets.Should().Contain(textWidget);
        }

        [Fact]
        public void WebpageWidgetAdminService_HideWidget_RemovesAWidgetFromTheShownListIfItIsIncluded()
        {

            var textWidget = new BasicMappedWidget();
            Session.Transact(session => session.Save(textWidget));

            var textPage = new StubWebpage
            {
                ShownWidgets = new HashSet<Widget> { textWidget },
                HiddenWidgets = new HashSet<Widget>()
            };
            Session.Transact(session => session.Save(textPage));

            _webpageWidgetAdminService.Hide(textPage, textWidget.Id);

            textPage.ShownWidgets.Should().NotContain(textWidget);
        }

        [Fact]
        public void WebpageWidgetAdminService_HideWidget_DoesNothingIfTheWidgetIdIsInvalid()
        {
            var textWidget = new BasicMappedWidget();
            Session.Transact(session => session.Save(textWidget));

            var textPage = new StubWebpage
            {
                ShownWidgets = new HashSet<Widget> { textWidget },
                HiddenWidgets = new HashSet<Widget>()
            };
            Session.Transact(session => session.Save(textPage));

            _webpageWidgetAdminService.Hide(textPage, -1);

            textPage.ShownWidgets.Should().Contain(textWidget);
        }


        [Fact]
        public void WebpageWidgetAdminService_ShowWidget_AddsAWidgetToTheShownWidgetsListIfItIsNotInTheHiddenList()
        {
            var textPage = new StubWebpage { ShownWidgets = new HashSet<Widget>(), HiddenWidgets = new HashSet<Widget>() };
            Session.Transact(session => session.Save(textPage));

            var textWidget = new BasicMappedWidget();
            Session.Transact(session => session.Save(textWidget));

            _webpageWidgetAdminService.Show(textPage, textWidget.Id);

            textPage.ShownWidgets.Should().Contain(textWidget);
        }

        [Fact]
        public void WebpageWidgetAdminService_ShowWidget_RemovesAWidgetFromTheHiddenListIfItIsIncluded()
        {
            var textWidget = new BasicMappedWidget();
            Session.Transact(session => session.Save(textWidget));

            var textPage = new StubWebpage
            {
                ShownWidgets = new HashSet<Widget>(),
                HiddenWidgets = new HashSet<Widget> { textWidget }
            };
            Session.Transact(session => session.Save(textPage));

            _webpageWidgetAdminService.Show(textPage, textWidget.Id);

            textPage.HiddenWidgets.Should().NotContain(textWidget);
        }

        [Fact]
        public void WebpageWidgetAdminService_ShowWidget_DoesNothingIfTheWidgetIdIsInvalid()
        {
            var textWidget = new BasicMappedWidget();
            Session.Transact(session => session.Save(textWidget));

            var textPage = new StubWebpage
            {
                ShownWidgets = new HashSet<Widget>(),
                HiddenWidgets = new HashSet<Widget> { textWidget }
            };
            Session.Transact(session => session.Save(textPage));

            _webpageWidgetAdminService.Show(textPage, -1);

            textPage.HiddenWidgets.Should().Contain(textWidget);
        }

        [Fact]
        public void WebpageWidgetAdminService_HideWidget_CallsSaveDocumentOnWebpage()
        {
            var textPage = new StubWebpage { ShownWidgets = new HashSet<Widget>(), HiddenWidgets = new HashSet<Widget>() };
            Session.Transact(session => session.Save(textPage));

            var textWidget = new BasicMappedWidget();
            Session.Transact(session => session.Save(textWidget));

            _webpageWidgetAdminService.Hide(textPage, textWidget.Id);

            A.CallTo(() => _documentService.SaveDocument<Webpage>(textPage)).MustHaveHappened();
        }

        [Fact]
        public void WebpageWidgetAdminService_ShowWidget_CallsSaveDocumentOnWebpage()
        {
            var textPage = new StubWebpage { ShownWidgets = new HashSet<Widget>(), HiddenWidgets = new HashSet<Widget>() };
            Session.Transact(session => session.Save(textPage));

            var textWidget = new BasicMappedWidget();
            Session.Transact(session => session.Save(textWidget));

            _webpageWidgetAdminService.Show(textPage, textWidget.Id);

            A.CallTo(() => _documentService.SaveDocument<Webpage>(textPage)).MustHaveHappened();
        }
    }
}