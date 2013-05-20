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

            var site = new Site();
            Session.Transact(session => session.Save(site));
            var errorPageOptions = siteSettingsOptionGenerator.GetErrorPageOptions(Session, site, -1);

            errorPageOptions.Should().BeEmpty();
        }

        [Fact]
        public void SiteSettingsOptionGenerator_GetErrorPageOptions_IncludesSavedPublishedWebpages()
        {
            var siteSettingsOptionGenerator = new SiteSettingsOptionGenerator();

            var textPage = new BasicMappedWebpage { PublishOn = CurrentRequestData.Now.AddDays(-1), Name = "Test 1", Site = CurrentSite };
            Session.Transact(session => session.Save(textPage));

            var errorPageOptions = siteSettingsOptionGenerator.GetErrorPageOptions(Session, CurrentSite, -1);

            errorPageOptions.Should().HaveCount(1);
            errorPageOptions[0].Text.Should().Be("Test 1");
            errorPageOptions[0].Value.Should().Be(textPage.Id.ToString());
        }

        [Fact]
        public void SiteSettingsOptionGenerator_GetErrorPageOptions_ExcludesPagesForOtherSites()
        {
            var siteSettingsOptionGenerator = new SiteSettingsOptionGenerator();

            var textPage = new BasicMappedWebpage { PublishOn = CurrentRequestData.Now.AddDays(-1), Name = "Test 1", Site = CurrentSite };
            Session.Transact(session =>
                                 {
                                     session.Save(textPage);
                                 });
            var site2 = new Site();
            var textPage2 = new BasicMappedWebpage { PublishOn = CurrentRequestData.Now.AddDays(-1), Name = "Test 2", Site = site2 };

            CurrentRequestData.CurrentSite = site2;
            Session.Transact(session =>
                {
                    session.Save(textPage2);
                });

            var errorPageOptions = siteSettingsOptionGenerator.GetErrorPageOptions(Session, site2, -1);

            errorPageOptions.Should().HaveCount(1);
            errorPageOptions[0].Text.Should().Be("Test 2");
            errorPageOptions[0].Value.Should().Be(textPage2.Id.ToString());
        }

        [Fact]
        public void SiteSettingOptionGenerator_GetErrorPageOptions_ExcludesSavedUnpublishedWebpages()
        {
            var siteSettingsOptionGenerator = new SiteSettingsOptionGenerator();

            var site = new Site();
            var textPage = new BasicMappedWebpage { Name = "Test 1", Site = site };
            Session.Transact(session => session.Save(textPage));

            var errorPageOptions = siteSettingsOptionGenerator.GetErrorPageOptions(Session, site, -1);

            errorPageOptions.Should().HaveCount(0);
        }

        [Fact]
        public void SiteSettingsOptionGenerator_GetErrorPageOptions_ItemIsSelectedIfTheIdMatches()
        {
            var siteSettingsOptionGenerator = new SiteSettingsOptionGenerator();

            var textPage = new BasicMappedWebpage { PublishOn = CurrentRequestData.Now.AddDays(-1), Name = "Test 1", Site = CurrentSite };
            Session.Transact(session => session.Save(textPage));

            var errorPageOptions = siteSettingsOptionGenerator.GetErrorPageOptions(Session, CurrentSite, textPage.Id);

            errorPageOptions.Should().HaveCount(1);
            errorPageOptions[0].Selected.Should().BeTrue();
        }

        [Fact]
        public void SiteSettingsOptionGenerator_GetLayoutOptions_WithoutDefaultIncludedIsEmpty()
        {
            var siteSettingsOptionGenerator = new SiteSettingsOptionGenerator();

            var site = new Site();
            Session.Transact(session => session.Save(site));
            var errorPageOptions = siteSettingsOptionGenerator.GetLayoutOptions(Session, site, -1);

            errorPageOptions.Should().HaveCount(0);
        }

        [Fact]
        public void SiteSettingsOptionGenerator_GetLayoutOptions_IncludesLayouts()
        {
            var siteSettingsOptionGenerator = new SiteSettingsOptionGenerator();

            var layout = new Layout { Name = "Test Layout", Site = CurrentSite };
            Session.Transact(session => session.Save(layout));

            var errorPageOptions = siteSettingsOptionGenerator.GetLayoutOptions(Session, CurrentSite, -1);

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

            var errorPageOptions = siteSettingsOptionGenerator.GetLayoutOptions(Session, CurrentSite, layout2.Id);

            errorPageOptions.Should().HaveCount(2);
            errorPageOptions[0].Selected.Should().BeFalse();
            errorPageOptions[1].Selected.Should().BeTrue();
        }

        [Fact]
        public void SiteSettingsOptionGenerator_GetLayoutOptions_FiltersBySite()
        {
            var siteSettingsOptionGenerator = new SiteSettingsOptionGenerator();

            var site1 = new Site();
            var layout = new Layout { Name = "Test Layout", Site = site1 };
            Session.Transact(session =>
            {
                session.Save(layout);
            });

            var site2 = new Site();
            var layout2 = new Layout { Name = "Test Layout 2", Site = site2 };
            CurrentRequestData.CurrentSite = site2;
            Session.Transact(session =>
            {
                session.Save(layout2);
            });

            var errorPageOptions = siteSettingsOptionGenerator.GetLayoutOptions(Session, site2, layout2.Id);

            errorPageOptions.Should().HaveCount(1);
            errorPageOptions[0].Value.Should().Be(layout2.Id.ToString());
        }
    }
}