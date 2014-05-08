using System;
using FluentAssertions;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Settings;
using MrCMS.Tests.Stubs;
using MrCMS.Website;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.Tests.Settings
{
    public class SiteSettingsOptionGeneratorTests : InMemoryDatabaseTest
    {
        [Fact]
        public void SiteSettingsOptionGenerator_GetErrorPageOptions_IsEmptyWithNoPages()
        {
            var siteSettingsOptionGenerator = new SiteSettingsOptionGenerator();

            var errorPageOptions = siteSettingsOptionGenerator.GetErrorPageOptions(Session,  -1);

            errorPageOptions.Should().BeEmpty();
        }

        [Fact]
        public void SiteSettingsOptionGenerator_GetErrorPageOptions_IncludesSavedPublishedWebpages()
        {
            var siteSettingsOptionGenerator = new SiteSettingsOptionGenerator();

            var textPage = new BasicMappedWebpage { PublishOn = CurrentRequestData.Now.AddDays(-1), Name = "Test 1", Site = CurrentSite };
            Session.Transact(session => session.Save(textPage));

            var errorPageOptions = siteSettingsOptionGenerator.GetErrorPageOptions(Session, -1);

            errorPageOptions.Should().HaveCount(1);
            errorPageOptions[0].Text.Should().Be("Test 1");
            errorPageOptions[0].Value.Should().Be(textPage.Id.ToString());
        }
        [Fact]
        public void SiteSettingOptionGenerator_GetErrorPageOptions_ExcludesSavedUnpublishedWebpages()
        {
            var siteSettingsOptionGenerator = new SiteSettingsOptionGenerator();

            var textPage = new BasicMappedWebpage { Name = "Test 1"};
            Session.Transact(session => session.Save(textPage));

            var errorPageOptions = siteSettingsOptionGenerator.GetErrorPageOptions(Session, -1);

            errorPageOptions.Should().HaveCount(0);
        }

        [Fact]
        public void SiteSettingsOptionGenerator_GetErrorPageOptions_ItemIsSelectedIfTheIdMatches()
        {
            var siteSettingsOptionGenerator = new SiteSettingsOptionGenerator();

            var textPage = new BasicMappedWebpage { PublishOn = CurrentRequestData.Now.AddDays(-1), Name = "Test 1", Site = CurrentSite };
            Session.Transact(session => session.Save(textPage));

            var errorPageOptions = siteSettingsOptionGenerator.GetErrorPageOptions(Session,  textPage.Id);

            errorPageOptions.Should().HaveCount(1);
            errorPageOptions[0].Selected.Should().BeTrue();
        }

        [Fact]
        public void SiteSettingsOptionGenerator_GetLayoutOptions_WithoutDefaultIncludedIsEmpty()
        {
            var siteSettingsOptionGenerator = new SiteSettingsOptionGenerator();

            var errorPageOptions = siteSettingsOptionGenerator.GetLayoutOptions(Session, -1);

            errorPageOptions.Should().HaveCount(0);
        }

        [Fact]
        public void SiteSettingsOptionGenerator_GetLayoutOptions_IncludesLayouts()
        {
            var siteSettingsOptionGenerator = new SiteSettingsOptionGenerator();

            var layout = new Layout { Name = "Test Layout", Site = CurrentSite };
            Session.Transact(session => session.Save(layout));

            var errorPageOptions = siteSettingsOptionGenerator.GetLayoutOptions(Session, -1);

            errorPageOptions.Should().HaveCount(1);
            errorPageOptions[0].Text.Should().Be("Test Layout");
            errorPageOptions[0].Value.Should().Be(layout.Id.ToString());
        }

        [Fact]
        public void SiteSettingsOptionGenerator_GetLayoutOptions_IfLayoutIdIsPassedFlagIsTrue()
        {
            var siteSettingsOptionGenerator = new SiteSettingsOptionGenerator();

            var layout = new Layout { Name = "Test Layout", Site = CurrentSite };
            var layout2 = new Layout { Name = "Test Layout 2", Site = CurrentSite };
            Session.Transact(session =>
            {
                session.Save(layout);
                session.Save(layout2);
            });

            var errorPageOptions = siteSettingsOptionGenerator.GetLayoutOptions(Session, layout2.Id);

            errorPageOptions.Should().HaveCount(2);
            errorPageOptions[0].Selected.Should().BeFalse();
            errorPageOptions[1].Selected.Should().BeTrue();
        }
    }
}