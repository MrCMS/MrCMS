using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Paging;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Web.Tests.Stubs;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class VersionsControllerTests
    {
        private readonly IDocumentVersionsAdminService _documentVersionsAdminService;
        private readonly VersionsController _versionsController;

        public VersionsControllerTests()
        {
            _documentVersionsAdminService = A.Fake<IDocumentVersionsAdminService>();
            _versionsController = new VersionsController(_documentVersionsAdminService);
        }

        [Fact]
        public void VersionsController_Show_ReturnsPartialViewResult()
        {
            var document = new StubDocument();
            _versionsController.Show(document, 1).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void VersionsController_Show_CallsTheGetVersionsMethodOnTheService()
        {
            var document = new StubDocument();

            _versionsController.Show(document, 1);

            A.CallTo(() => _documentVersionsAdminService.GetVersions(document, 1)).MustHaveHappened();
        }

        [Fact]
        public void VersionsController_Show_ReturnsTheResultOfTheServiceCallAsTheModel()
        {
            var document = new StubDocument();
            var versionsModel = new VersionsModel(PagedList<DocumentVersion>.Empty, document.Id);
            A.CallTo(() => _documentVersionsAdminService.GetVersions(document, 1)).Returns(versionsModel);

            PartialViewResult result = _versionsController.Show(document, 1);

            result.Model.Should().Be(versionsModel);
        }
    }
}