using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Controllers;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using Xunit;

namespace MrCMS.Web.Apps.Admin.Tests.Controllers
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
        public void SitesController_IndexGet_CallsISiteServiceGetAllSites()
        {
            _sitesController.Index_Get();

            A.CallTo(() => _siteAdminService.GetAllSites()).MustHaveHappened();
        }

        [Fact]
        public void SitesController_IndexGet_IfSitesReturnsViewIndexWithResultOfServiceCallAsModel()
        {
            var sites = new List<Site> { new Site() };
            A.CallTo(() => _siteAdminService.GetAllSites()).Returns(sites);

            ViewResult result = _sitesController.Index_Get();

            result.ViewName.Should().Be("Index");
            result.Model.Should().Be(sites);
        }

        [Fact]
        public void SitesController_AddPost_CallsSiteServiceSaveSiteWithPassedModel()
        {
            var model = new AddSiteModel();
            var options = new List<SiteCopyOption>();

            _sitesController.Add(model, options);

            A.CallTo(() => _siteAdminService.AddSite(model, options)).MustHaveHappened();
        }

        [Fact]
        public void SitesController_AddPost_RedirectsToIndex()
        {
            var model = new AddSiteModel();
            var options = new List<SiteCopyOption>(); 

            var result = _sitesController.Add(model, options);

            result.ActionName.Should().Be("Index");
        }

        [Fact]
        public void SitesController_EditGet_ReturnsViewResultWithLoadedUpdateModelAsViewModel()
        {
            var model = new UpdateSiteModel();
            A.CallTo(() => _siteAdminService.GetEditModel(123)).Returns(model);

            var result = _sitesController.Edit_Get(123);

            result.Should().NotBeNull();
            result.Model.Should().Be(model);
        }

        [Fact]
        public void SitesController_EditPost_CallsSaveSiteWithPassedModel()
        {
            var model = new UpdateSiteModel();

            _sitesController.Edit(model);

            A.CallTo(() => _siteAdminService.SaveSite(model)).MustHaveHappened();
        }

        [Fact]
        public void SitesController_EditPost_RedirectsToIndex()
        {
            var model = new UpdateSiteModel();

            var result = _sitesController.Edit(model);

            result.ActionName.Should().Be("Index");
        }

        [Fact]
        public void SitesController_DeleteGet_ReturnsAPartialViewResultWithUpdateModelAsViewModel()
        {
            var model = new UpdateSiteModel();
            A.CallTo(() => _siteAdminService.GetEditModel(123)).Returns(model);

            PartialViewResult result = _sitesController.Delete_Get(123);

            result.Should().NotBeNull();
            result.Model.Should().Be(model);
        }

        [Fact]
        public void SitesController_DeletePost_CallsDeleteSiteOnSiteService()
        {
            _sitesController.Delete(123);

            A.CallTo(() => _siteAdminService.DeleteSite(123)).MustHaveHappened();
        }

        [Fact]
        public void SitesController_DeletePost_RedirectsToIndex()
        {
            var result = _sitesController.Delete(123);

            result.ActionName.Should().Be("Index");
        }
    }
}