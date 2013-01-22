using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Multisite;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Areas.Admin.Controllers;
using NHibernate;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class SitesControllerTests
    {
        private ISiteService _siteService;
        private IUserService _userService;
        private IConfigurationProvider configurationProvider;

        [Fact]
        public void SitesController_IndexGet_CallsISiteServiceGetAllSites()
        {
            SitesController sitesController = GetSitesController();

            sitesController.Index_Get();

            A.CallTo(() => _siteService.GetAllSites()).MustHaveHappened();
        }

        [Fact]
        public void SitesController_IndexGet_IfSitesReturnsViewIndexWithResultOfServiceCallAsModel()
        {
            SitesController sitesController = GetSitesController();
            var sites = new List<Site> {new Site()};
            A.CallTo(() => _siteService.GetAllSites()).Returns(sites);

            ViewResult result = sitesController.Index_Get();

            result.ViewName.Should().Be("Index");
            result.Model.Should().Be(sites);
        }

        [Fact]
        public void SitesController_AddGet_ReturnsPartialViewResultWithModelOfTypeSite()
        {
            SitesController sitesController = GetSitesController();

            PartialViewResult result = sitesController.Add_Get();

            result.Should().NotBeNull();
            result.Model.Should().BeOfType<Site>();
        }

        [Fact]
        public void SitesController_AddPost_CallsSiteServiceSaveSiteWithPassedSite()
        {
            SitesController sitesController = GetSitesController();
            var site = new Site();
            sitesController.Add(site);

            A.CallTo(() => _siteService.AddSite(site)).MustHaveHappened();
        }

        [Fact]
        public void SitesController_AddPost_RedirectsToIndex()
        {
            SitesController sitesController = GetSitesController();
            var site = new Site();
            RedirectToRouteResult result = sitesController.Add(site);

            result.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void SitesController_EditGet_ReturnsViewResultWithPassedSiteAsModel()
        {
            SitesController sitesController = GetSitesController();

            var site = new Site();
            ViewResult result = sitesController.Edit_Get(site);

            result.Should().NotBeNull();
            result.Model.Should().Be(site);
        }

        [Fact]
        public void SitesController_EditPost_CallsSaveSiteWithPassedSite()
        {
            SitesController sitesController = GetSitesController();

            var site = new Site();
            sitesController.Edit(site);

            A.CallTo(() => _siteService.SaveSite(site)).MustHaveHappened();
        }

        [Fact]
        public void SitesController_EditPost_RedirectsToIndex()
        {
            SitesController sitesController = GetSitesController();

            var site = new Site();
            RedirectToRouteResult result = sitesController.Edit(site);

            result.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void SitesController_DeleteGet_ReturnsAPartialViewResultWithPassedModel()
        {
            SitesController sitesController = GetSitesController();
            var site = new Site();
            PartialViewResult result = sitesController.Delete_Get(site);

            result.Should().NotBeNull();
            result.Model.Should().Be(site);
        }

        [Fact]
        public void SitesController_DeletePost_CallsDeleteSiteOnSiteService()
        {
            SitesController sitesController = GetSitesController();
            var site = new Site();
            sitesController.Delete(site);

            A.CallTo(() => _siteService.DeleteSite(site)).MustHaveHappened();
        }

        [Fact]
        public void SitesController_DeletePost_RedirectsToIndex()
        {
            SitesController sitesController = GetSitesController();
            var site = new Site();
            RedirectToRouteResult result = sitesController.Delete(site);

            result.RouteValues["action"].Should().Be("Index");
        }

        private SitesController GetSitesController()
        {
            _siteService = A.Fake<ISiteService>();
            configurationProvider = A.Fake<IConfigurationProvider>();
            _userService = A.Fake<IUserService>();
            return new SitesController( _siteService, _userService, configurationProvider);
        }
    }
}