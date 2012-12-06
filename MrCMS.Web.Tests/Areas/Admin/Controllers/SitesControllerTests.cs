using System.Collections.Generic;
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
        private ISitesService sitesService;
        private ISession session;
        private IConfigurationProvider configurationProvider;

        [Fact]
        public void SitesController_IndexGet_CallsISiteServiceGetAllSites()
        {
            var sitesController = GetSitesController();

            sitesController.Index_Get();

            A.CallTo(() => sitesService.GetAllSites()).MustHaveHappened();
        }

        [Fact]
        public void SitesController_IndexGet_IfSitesReturnsViewIndexWithResultOfServiceCallAsModel()
        {
            var sitesController = GetSitesController();
            var sites = new List<Site> { new Site() };
            A.CallTo(() => sitesService.GetAllSites()).Returns(sites);

            var result = sitesController.Index_Get();

            result.ViewName.Should().Be("Index");
            result.Model.Should().Be(sites);
        }

        [Fact]
        public void SitesController_AddGet_ReturnsPartialViewResultWithModelOfTypeSite()
        {
            var sitesController = GetSitesController();

            var result = sitesController.Add_Get();

            result.Should().NotBeNull();
            result.Model.Should().BeOfType<Site>();
        }

        [Fact]
        public void SitesController_AddPost_CallsSiteServiceSaveSiteWithPassedSite()
        {
            var sitesController = GetSitesController();
            var site = new Site();
            sitesController.Add(site);

            A.CallTo(() => sitesService.SaveSite(site)).MustHaveHappened();
        }

        [Fact]
        public void SitesController_AddPost_RedirectsToIndex()
        {
            var sitesController = GetSitesController();
            var site = new Site();
            var result = sitesController.Add(site);

            result.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void SitesController_EditGet_ReturnsViewResultWithPassedSiteAsModel()
        {
            var sitesController = GetSitesController();

            var site = new Site();
            var result = sitesController.Edit_Get(site);

            result.Should().NotBeNull();
            result.Model.Should().Be(site);
        }

        [Fact]
        public void SitesController_EditGet_SettingServiceGetAllSettingsForSiteIdShouldBeCalled()
        {
            var sitesController = GetSitesController();

            var site = new Site {Id = 1};
            sitesController.Edit_Get(site);

            A.CallTo(() => configurationProvider.GetAllISettings(site)).MustHaveHappened();
        }

        [Fact]
        public void SitesController_EditGet_ResultOfGetAllISettingsMustBeSetToViewDataSettings()
        {
            var sitesController = GetSitesController();
            var site = new Site {Id = 1};
            var settingses = new List<ISettings> {new SiteSettings()};
            A.CallTo(() => configurationProvider.GetAllISettings(site)).Returns(settingses);

            sitesController.Edit_Get(site);

            sitesController.ViewData["Settings"].As<List<ISettings>>().Should().BeEquivalentTo(settingses);
        }

        [Fact]
        public void SitesController_EditPost_CallsSaveSiteWithPassedSite()
        {
            var sitesController = GetSitesController();

            var site = new Site();
            sitesController.Edit(site,new List<ISettings>());

            A.CallTo(() => sitesService.SaveSite(site)).MustHaveHappened();
        }

        [Fact]
        public void SitesController_EditPost_RedirectsToIndex()
        {
            var sitesController = GetSitesController();

            var site = new Site();
            var result = sitesController.Edit(site, new List<ISettings>());

            result.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void SitesController_DeleteGet_ReturnsAPartialViewResultWithPassedModel()
        {
            var sitesController = GetSitesController();
            var site = new Site();
            var result = sitesController.Delete_Get(site);

            result.Should().NotBeNull();
            result.Model.Should().Be(site);
        }

        [Fact]
        public void SitesController_DeletePost_CallsDeleteSiteOnSiteService()
        {
            var sitesController = GetSitesController();
            var site = new Site();
            sitesController.Delete(site);

            A.CallTo(() => sitesService.DeleteSite(site)).MustHaveHappened();
        }

        [Fact]
        public void SitesController_DeletePost_RedirectsToIndex()
        {
            var sitesController = GetSitesController();
            var site = new Site();
            var result = sitesController.Delete(site);

            result.RouteValues["action"].Should().Be("Index");
        }

        private SitesController GetSitesController()
        {
            session = A.Fake<ISession>();
            sitesService = A.Fake<ISitesService>();
            configurationProvider = A.Fake<IConfigurationProvider>();
            return new SitesController(session, sitesService, configurationProvider);
        }
    }
}