using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Models;
using MrCMS.Web.Admin.Controllers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;
using Xunit;

namespace MrCMS.Web.Admin.Tests.Controllers
{
    public class SitesControllerTests
    {
        private readonly ISiteAdminService _siteAdminService;
        private readonly SitesController _sitesController;

        public SitesControllerTests()
        {
            _siteAdminService = A.Fake<ISiteAdminService>();
            _sitesController = new SitesController(_siteAdminService);
        }

        [Fact]
        public async Task SitesController_IndexGet_CallsISiteServiceGetAllSites()
        {
            await _sitesController.Index_Get();

            A.CallTo(() => _siteAdminService.GetAllSites()).MustHaveHappened();
        }

        [Fact]
        public async Task SitesController_IndexGet_IfSitesReturnsViewIndexWithResultOfServiceCallAsModel()
        {
            var sites = new List<Site> { new Site() };
            A.CallTo(() => _siteAdminService.GetAllSites()).Returns(sites);

            ViewResult result = await _sitesController.Index_Get();

            result.ViewName.Should().Be("Index");
            result.Model.Should().Be(sites);
        }

        [Fact]
        public async Task SitesController_AddPost_CallsSiteServiceSaveSiteWithPassedModel()
        {
            var model = new AddSiteModel();
            var options = new List<SiteCopyOption>();

           await  _sitesController.Add(model, options);

            A.CallTo(() => _siteAdminService.AddSite(model, options)).MustHaveHappened();
        }

        [Fact]
        public async Task SitesController_AddPost_RedirectsToIndex()
        {
            var model = new AddSiteModel();
            var options = new List<SiteCopyOption>(); 

            var result =await _sitesController.Add(model, options);

            result.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task SitesController_EditGet_ReturnsViewResultWithLoadedUpdateModelAsViewModel()
        {
            var model = new UpdateSiteModel();
            A.CallTo(() => _siteAdminService.GetEditModel(123)).Returns(model);

            var result = await _sitesController.Edit_Get(123);

            result.Should().NotBeNull();
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task SitesController_EditPost_CallsSaveSiteWithPassedModel()
        {
            var model = new UpdateSiteModel();

           await _sitesController.Edit(model);

            A.CallTo(() => _siteAdminService.SaveSite(model)).MustHaveHappened();
        }

        [Fact]
        public async Task SitesController_EditPost_RedirectsToIndex()
        {
            var model = new UpdateSiteModel();

            var result =await _sitesController.Edit(model);

            result.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task SitesController_DeleteGet_ReturnsAPartialViewResultWithUpdateModelAsViewModel()
        {
            var model = new UpdateSiteModel();
            A.CallTo(() => _siteAdminService.GetEditModel(123)).Returns(model);

            PartialViewResult result =await _sitesController.Delete_Get(123);

            result.Should().NotBeNull();
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task SitesController_DeletePost_CallsDeleteSiteOnSiteService()
        {
          await  _sitesController.Delete(123);

            A.CallTo(() => _siteAdminService.DeleteSite(123)).MustHaveHappened();
        }

        [Fact]
        public async Task SitesController_DeletePost_RedirectsToIndex()
        {
            var result = await _sitesController.Delete(123);

            result.ActionName.Should().Be("Index");
        }
    }
}