using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Multisite;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Controllers;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class SitesControllerTests
    {
        private readonly ISiteService _siteService;
        private readonly SitesController _sitesController;

        public SitesControllerTests()
        {
            _siteService = A.Fake<ISiteService>();
            _sitesController = new SitesController(_siteService);
        }

        [Fact]
        public void SitesController_IndexGet_CallsISiteServiceGetAllSites()
        {
            _sitesController.Index_Get();

            A.CallTo(() => _siteService.GetAllSites()).MustHaveHappened();
        }

        [Fact]
        public void SitesController_IndexGet_IfSitesReturnsViewIndexWithResultOfServiceCallAsModel()
        {
            var sites = new List<Site> { new Site() };
            A.CallTo(() => _siteService.GetAllSites()).Returns(sites);

            ViewResult result = _sitesController.Index_Get();

            result.ViewName.Should().Be("Index");
            result.Model.Should().Be(sites);
        }

        [Fact]
        public void SitesController_AddGet_ReturnsPartialViewResultWithModelOfTypeSite()
        {
            PartialViewResult result = _sitesController.Add_Get();

            result.Should().NotBeNull();
            result.Model.Should().BeOfType<Site>();
        }

        [Fact]
        public void SitesController_AddPost_CallsSiteServiceSaveSiteWithPassedSite()
        {
            var site = new Site();
            var options = new SiteCopyOptions();

            _sitesController.Add(site, options);

            A.CallTo(() => _siteService.AddSite(site, options)).MustHaveHappened();
        }

        [Fact]
        public void SitesController_AddPost_RedirectsToIndex()
        {
            var site = new Site();
            var options = new SiteCopyOptions();

            RedirectToRouteResult result = _sitesController.Add(site, options);

            result.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void SitesController_EditGet_ReturnsViewResultWithPassedSiteAsModel()
        {
            var site = new Site();

            ViewResult result = _sitesController.Edit_Get(site);

            result.Should().NotBeNull();
            result.Model.Should().Be(site);
        }

        [Fact]
        public void SitesController_EditPost_CallsSaveSiteWithPassedSite()
        {
            var site = new Site();

            _sitesController.Edit(site);

            A.CallTo(() => _siteService.SaveSite(site)).MustHaveHappened();
        }

        [Fact]
        public void SitesController_EditPost_RedirectsToIndex()
        {
            var site = new Site();

            RedirectToRouteResult result = _sitesController.Edit(site);

            result.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void SitesController_DeleteGet_ReturnsAPartialViewResultWithPassedModel()
        {
            var site = new Site();

            PartialViewResult result = _sitesController.Delete_Get(site);

            result.Should().NotBeNull();
            result.Model.Should().Be(site);
        }

        [Fact]
        public void SitesController_DeletePost_CallsDeleteSiteOnSiteService()
        {
            var site = new Site();

            _sitesController.Delete(site);

            A.CallTo(() => _siteService.DeleteSite(site)).MustHaveHappened();
        }

        [Fact]
        public void SitesController_DeletePost_RedirectsToIndex()
        {
            var site = new Site();

            RedirectToRouteResult result = _sitesController.Delete(site);

            result.RouteValues["action"].Should().Be("Index");
        }
    }
}