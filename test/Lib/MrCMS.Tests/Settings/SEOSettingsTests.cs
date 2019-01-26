using System;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MrCMS.Settings;
using MrCMS.TestSupport;
using NHibernate;
using Xunit;

namespace MrCMS.Tests.Settings
{
    public class SEOSettingsTests
    {
        [Fact]
        public void SEOSettings_GetTypeName_ReturnsSEOSettings()
        {
            var seoSettings = new SEOSettings();

            seoSettings.TypeName.Should().Be("SEO Settings");
        }

        [Fact]
        public void SEOSettings_GetDivId_ReturnsSEODashSettings()
        {
            var seoSettings = new SEOSettings();

            seoSettings.DivId.Should().Be("seo-settings");
        }

        [Fact]
        public void SEOSettings_SetViewData_ShouldNotThrow()
        {
            var seoSettings = new SEOSettings();
            this.Invoking(tests =>
                    seoSettings.SetViewData(A.Fake<IServiceProvider>(), ViewDataDictionaryHelper.GetNewDictionary())).Should()
                .NotThrow();
        }
    }
}