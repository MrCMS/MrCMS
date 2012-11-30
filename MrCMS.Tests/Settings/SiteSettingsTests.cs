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
        [Fact]
        public void SiteSettings_GetTypeName_ReturnsSiteSettings()
        {
            var siteSettings = new SiteSettings();

            siteSettings.GetTypeName().Should().Be("Site Settings");
        }

        [Fact]
        public void SiteSettings_GetDivId_ReturnsSiteDashSettings()
        {
            var siteSettings = new SiteSettings();

            siteSettings.GetDivId().Should().Be("site-settings");
        }

        [Fact]
        public void SiteSettings_SetViewData_ShouldNotThrow()
        {
            var siteSettings = new SiteSettings();
            this.Invoking(tests =>
                          siteSettings.SetViewData(A.Fake<ISession>(), A.Fake<ViewDataDictionary>())).ShouldNotThrow();
        }
    }
}