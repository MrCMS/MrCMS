using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Settings;
using NHibernate;
using Xunit;

namespace MrCMS.Tests.Settings
{
    public class SiteSettingsTests
    {
        public SiteSettingsTests()
        {
            _siteSettings = new SiteSettings();
            _siteSettingsOptionGenerator = A.Fake<SiteSettingsOptionGenerator>();
            _siteSettings.SetSiteSettingsOptionGeneratorOverride(_siteSettingsOptionGenerator);
        }

        private readonly SiteSettings _siteSettings;
        private readonly SiteSettingsOptionGenerator _siteSettingsOptionGenerator;

        [Fact]
        public void SiteSettings_GetTypeName_ReturnsSiteSettings()
        {
            _siteSettings.TypeName.Should().Be("Site Settings");
        }

        [Fact]
        public void SiteSettings_GetDivId_ReturnsSiteDashSettings()
        {
            _siteSettings.DivId.Should().Be("site-settings");
        }

        [Fact]
        public void SiteSettings_SetViewData_ShouldAssignTheResultOfSiteSettingsOptionGeneratorGetLayoutOptionsToDefaultLayoutOptionsViewData()
        {
            var session = A.Fake<ISession>();
            var viewDataDictionary = new ViewDataDictionary();
            var items = new List<SelectListItem>();
            A.CallTo(
                () =>
                _siteSettingsOptionGenerator.GetLayoutOptions(session, _siteSettings.Site, _siteSettings.DefaultLayoutId))
             .Returns(items);

            _siteSettings.SetViewData(session, viewDataDictionary);

            viewDataDictionary["DefaultLayoutOptions"].Should().Be(items);
        }

        [Fact]
        public void SiteSettings_SetViewData_ShouldAssignTheResultOfSiteSettingsOptionGeneratorGetErrorPageOptionsTo403OptionsViewData()
        {
            _siteSettings.Error403PageId = 123;
            var session = A.Fake<ISession>();
            var viewDataDictionary = new ViewDataDictionary();
            var items = new List<SelectListItem>();
            A.CallTo(
                () =>
                _siteSettingsOptionGenerator.GetErrorPageOptions(session, _siteSettings.Site, _siteSettings.Error403PageId))
             .Returns(items);

            _siteSettings.SetViewData(session, viewDataDictionary);

            viewDataDictionary["403Options"].Should().Be(items);
        }

        [Fact]
        public void SiteSettings_SetViewData_ShouldAssignTheResultOfSiteSettingsOptionGeneratorGetErrorPageOptionsTo404OptionsViewData()
        {
            _siteSettings.Error404PageId = 123;
            var session = A.Fake<ISession>();
            var viewDataDictionary = new ViewDataDictionary();
            var items = new List<SelectListItem>();
            A.CallTo(
                () =>
                _siteSettingsOptionGenerator.GetErrorPageOptions(session, _siteSettings.Site, _siteSettings.Error404PageId))
             .Returns(items);

            _siteSettings.SetViewData(session, viewDataDictionary);

            viewDataDictionary["404Options"].Should().Be(items);
        }

        [Fact]
        public void SiteSettings_SetViewData_ShouldAssignTheResultOfSiteSettingsOptionGeneratorGetErrorPageOptionsTo500OptionsViewData()
        {
            _siteSettings.Error500PageId = 123;
            var session = A.Fake<ISession>();
            var viewDataDictionary = new ViewDataDictionary();
            var items = new List<SelectListItem>();
            A.CallTo(
                () =>
                _siteSettingsOptionGenerator.GetErrorPageOptions(session, _siteSettings.Site, _siteSettings.Error500PageId))
             .Returns(items);

            _siteSettings.SetViewData(session, viewDataDictionary);

            viewDataDictionary["500Options"].Should().Be(items);
        }

        [Fact]
        public void SiteSettings_CultureInfo_IfUICultureIsNotSetShouldBeSystemCulture()
        {
            _siteSettings.CultureInfo.Should().Be(CultureInfo.CurrentCulture);
        }

        [Fact]
        public void SiteSettings_CultureInfo_IfUICultureIsSetShouldBeLoadedFromString()
        {
            _siteSettings.UICulture = "de";

            _siteSettings.CultureInfo.Should().Be(CultureInfo.GetCultureInfo("de"));
        }

        [Fact]
        public void SiteSettings_TimeZoneInfo_IfTimeZoneIsNotSetShouldBeTimeZoneLocal()
        {
            _siteSettings.TimeZoneInfo.Should().Be(TimeZoneInfo.Local);
        }

        [Fact]
        public void SiteSettings_TimeZoneInfo_GetsOffsetFromTimeZoneId()
        {
            _siteSettings.TimeZone = "UTC+12";

            _siteSettings.TimeZoneInfo.Should().Be(TimeZoneInfo.FindSystemTimeZoneById("UTC+12"));
        }
    }
}