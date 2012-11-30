using System;
using FluentAssertions;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Settings;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.Tests.Settings
{
    public class SiteSettingsOptionGeneratorTests : InMemoryDatabaseTest
    {
        [Fact]
        public void SiteSettingsOptionGenerator_GetErrorPageOptions_IncludesSelectPage()
        {
            var siteSettingsOptionGenerator = new SiteSettingsOptionGenerator();

            var errorPageOptions = siteSettingsOptionGenerator.GetErrorPageOptions(Session, -1);

            errorPageOptions.Should().HaveCount(1);
            errorPageOptions[0].Text.Should().Be("Select page");
        }

        [Fact]
        public void SiteSettingsOptionGenerator_GetErrorPageOptions_IncludesSavedPublishedWebpages()
        {
            var siteSettingsOptionGenerator = new SiteSettingsOptionGenerator();

            var textPage = new TextPage {PublishOn = DateTime.UtcNow.AddDays(-1), Name = "Test 1"};
            Session.Transact(session => session.Save(textPage));

            var errorPageOptions = siteSettingsOptionGenerator.GetErrorPageOptions(Session, -1);

            errorPageOptions.Should().HaveCount(2);
            errorPageOptions[1].Text.Should().Be("Test 1");
            errorPageOptions[1].Value.Should().Be(textPage.Id.ToString());
        }

        [Fact]
        public void SiteSettingOptionGenerator_GetErrorPageOptions_ExcludesSavedUnpublishedWebpages()
        {
            var siteSettingsOptionGenerator = new SiteSettingsOptionGenerator();

            var textPage = new TextPage { Name = "Test 1"};
            Session.Transact(session => session.Save(textPage));

            var errorPageOptions = siteSettingsOptionGenerator.GetErrorPageOptions(Session, -1);

            errorPageOptions.Should().HaveCount(1);
        }

        [Fact]
        public void SiteSettingsOptionGenerator_GetErrorPageOptions_ItemIsSelectedIfTheIdMatches()
        {
            var siteSettingsOptionGenerator = new SiteSettingsOptionGenerator();

            var textPage = new TextPage { PublishOn = DateTime.UtcNow.AddDays(-1), Name = "Test 1" };
            Session.Transact(session => session.Save(textPage));

            var errorPageOptions = siteSettingsOptionGenerator.GetErrorPageOptions(Session, textPage.Id);

            errorPageOptions.Should().HaveCount(2);
            errorPageOptions[1].Selected.Should().BeTrue();
        }

        [Fact]
        public void SiteSettingsOptionGenerator_GetLayoutOptions_WithoutDefaultIncludedIsEmpty()
        {
            var siteSettingsOptionGenerator = new SiteSettingsOptionGenerator();

            var errorPageOptions = siteSettingsOptionGenerator.GetLayoutOptions(Session, -1);

            errorPageOptions.Should().HaveCount(0);
        }

        [Fact]
        public void SiteSettingsOptionGenerator_GetLayoutOptions_WithDefaultIncludedIncludesDefaultLayout()
        {
            var siteSettingsOptionGenerator = new SiteSettingsOptionGenerator();

            var errorPageOptions = siteSettingsOptionGenerator.GetLayoutOptions(Session, -1, true);

            errorPageOptions.Should().HaveCount(1);
            errorPageOptions[0].Text.Should().Be("Default Layout");
        }

        [Fact]
        public void SiteSettingsOptionGenerator_GetLayoutOptions_IncludesLayouts()
        {
            var siteSettingsOptionGenerator = new SiteSettingsOptionGenerator();

            var layout = new Layout {Name = "Test Layout"};
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

            var layout = new Layout {Name = "Test Layout"};
            var layout2 = new Layout {Name = "Test Layout 2"};
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