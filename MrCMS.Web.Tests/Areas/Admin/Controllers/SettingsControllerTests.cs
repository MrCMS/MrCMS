using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Areas.Admin.Controllers;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class SettingsControllerTests
    {
        //[Fact]
        //public void SettingsController_IndexGet_CallsTheSettingServiceGetSettings()
        //{
        //    var siteSettingsService = A.Fake<ISiteSettingsService>();
        //    var settingsController = new SettingsController(siteSettingsService) {IsAjaxRequest=false};
        //    settingsController.Index();

        //    A.CallTo(() => siteSettingsService.GetAllSettings()).MustHaveHappened();
        //}

        //[Fact]
        //public void SettingsController_IndexGet_ModelShouldBeTheResultOfTheCallToSettingsService()
        //{
        //    var siteSettings = new SiteSettings();
        //    var mediaSettings = new MediaSettings();

        //    var siteSettingsService = A.Fake<ISiteSettingsService>();
        //    var settingsController = new SettingsController(siteSettings, mediaSettings) { IsAjaxRequest = false };
        //    A.CallTo(() => siteSettingsService.GetAllSettings()).Returns(siteSettings);

        //    var result = settingsController.Index();

        //    result.Model.Should().Be(siteSettings);
        //}

        //[Fact]
        //public void SettingsController_IndexPost_ShouldCallSettingServiceSaveMethod()
        //{
        //    var siteSettings = new SiteSettings();
        //    var siteSettingsService = A.Fake<ISiteSettingsService>();
        //    var settingsController = new SettingsController(siteSettingsService) {IsAjaxRequest=false};

        //    settingsController.Index(siteSettings);

        //    A.CallTo(()=>siteSettingsService.SaveSettings(siteSettings)).MustHaveHappened();
        //}
        //[Fact]
        //public void SettingsController_IndexPost_ShouldRedirectToHomeIndex()
        //{
        //    var siteSettings = new SiteSettings();
        //    var siteSettingsService = A.Fake<ISiteSettingsService>();
        //    var settingsController = new SettingsController(siteSettingsService) {IsAjaxRequest=false};

        //    var result = settingsController.Index(siteSettings);

        //    result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        //    result.As<RedirectToRouteResult>().RouteValues["controller"].Should().Be("Settings");
        //}
    }
}