using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using MrCMS.TestSupport;
using Xunit;

namespace MrCMS.Tests.Settings
{
    public class SiteSettingsTests
    {
        private readonly SiteSettings _siteSettings;

        private readonly ISiteSettingsOptionGenerator _siteSettingsOptionGenerator =
            A.Fake<ISiteSettingsOptionGenerator>();
        private readonly IServiceProvider _serviceProvider = A.Fake<IServiceProvider>();

        public SiteSettingsTests()
        {
            A.CallTo(() => _serviceProvider.GetService(typeof(ISiteSettingsOptionGenerator)))
                .Returns(_siteSettingsOptionGenerator);
            _siteSettings = new SiteSettings();
        }

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
            var items = new List<SelectListItem>();
            A.CallTo(() => _siteSettingsOptionGenerator.GetLayoutOptions(_siteSettings.DefaultLayoutId))
             .Returns(items);

            var viewDataDictionary = ViewDataDictionaryHelper.GetNewDictionary();
            _siteSettings.SetViewData(_serviceProvider, viewDataDictionary);

            viewDataDictionary["DefaultLayoutOptions"].Should().Be(items);
        }


        [Fact]
        public void SiteSettings_SetViewData_ShouldAssignTheResultOfSiteSettingsOptionGeneratorGetErrorPageOptionsTo403OptionsViewData()
        {
            _siteSettings.Error403PageId = 123;
            var items = new List<SelectListItem>();
            A.CallTo(() => _siteSettingsOptionGenerator.GetErrorPageOptions(_siteSettings.Error403PageId))
             .Returns(items);


            var viewDataDictionary = ViewDataDictionaryHelper.GetNewDictionary();
            _siteSettings.SetViewData(_serviceProvider, viewDataDictionary);
            viewDataDictionary["403Options"].Should().Be(items);
        }

        [Fact]
        public void SiteSettings_SetViewData_ShouldAssignTheResultOfSiteSettingsOptionGeneratorGetErrorPageOptionsTo404OptionsViewData()
        {
            _siteSettings.Error404PageId = 123;
            var items = new List<SelectListItem>();
            A.CallTo(() => _siteSettingsOptionGenerator.GetErrorPageOptions(_siteSettings.Error404PageId))
             .Returns(items);

            var viewDataDictionary = ViewDataDictionaryHelper.GetNewDictionary();
            _siteSettings.SetViewData(_serviceProvider, viewDataDictionary);

            viewDataDictionary["404Options"].Should().Be(items);
        }

        [Fact]
        public void SiteSettings_SetViewData_ShouldAssignTheResultOfSiteSettingsOptionGeneratorGetErrorPageOptionsTo500OptionsViewData()
        {
            _siteSettings.Error500PageId = 123;
            var items = new List<SelectListItem>();
            A.CallTo(() => _siteSettingsOptionGenerator.GetErrorPageOptions(_siteSettings.Error500PageId))
             .Returns(items);

            var viewDataDictionary = ViewDataDictionaryHelper.GetNewDictionary();
            _siteSettings.SetViewData(_serviceProvider, viewDataDictionary);


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

        /*[Fact]
        public void SiteSettings_TimeZoneInfo_IfTimeZoneIsNotSetShouldBeTimeZoneLocal()
        {
            _siteSettings.TimeZoneInfo.Should().Be(TimeZoneInfo.Local);
        }

        [Fact]
        public void SiteSettings_TimeZoneInfo_GetsOffsetFromTimeZoneSerialized()
        {
            _siteSettings.TimeZone =
                "GMT Standard Time;0;(UTC+00:00) Dublin, Edinburgh, Lisbon, London;GMT Standard Time;GMT Summer Time;[01:01:0001;12:31:9999;60;[0;01:00:00;3;5;0;];[0;02:00:00;10;5;0;];];";

            _siteSettings.TimeZoneInfo.StandardName.Should().Be("GMT Standard Time");
        }*/
    }
}