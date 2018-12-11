using FluentAssertions;
using MrCMS.Apps;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Tests.Stubs;
using System;
using MrCMS.TestSupport;
using Xunit;

namespace MrCMS.Tests.Settings
{
    public class SiteSettingsOptionGeneratorTests : InMemoryDatabaseTest
    {
        private readonly MrCMSAppContext _mrCMSAppContext = new MrCMSAppContext();
        private SiteSettingsOptionGenerator _sut;

        public SiteSettingsOptionGeneratorTests()
        {
            _sut = new SiteSettingsOptionGenerator(Session, _mrCMSAppContext);
        }
        [Fact]
        public void SiteSettingsOptionGenerator_GetErrorPageOptions_IsEmptyWithNoPages()
        {

            var errorPageOptions = _sut.GetErrorPageOptions(-1);

            errorPageOptions.Should().BeEmpty();
        }

        [Fact]
        public void SiteSettingsOptionGenerator_GetErrorPageOptions_IncludesSavedPublishedWebpages()
        {
            var textPage = new BasicMappedWebpage
            {
                PublishOn = DateTime.Now.AddDays(-1),
                Name = "Test 1",
                Site = CurrentSite,
                Published = true
            };
            Session.Transact(session => session.Save(textPage));

            var errorPageOptions = _sut.GetErrorPageOptions(-1);

            errorPageOptions.Should().HaveCount(1);
            errorPageOptions[0].Text.Should().Be("Test 1");
            errorPageOptions[0].Value.Should().Be(textPage.Id.ToString());
        }
        [Fact]
        public void SiteSettingOptionGenerator_GetErrorPageOptions_ExcludesSavedUnpublishedWebpages()
        {
            var textPage = new BasicMappedWebpage { Name = "Test 1" };
            Session.Transact(session => session.Save(textPage));

            var errorPageOptions = _sut.GetErrorPageOptions(-1);

            errorPageOptions.Should().HaveCount(0);
        }

        [Fact]
        public void SiteSettingsOptionGenerator_GetErrorPageOptions_ItemIsSelectedIfTheIdMatches()
        {
            var textPage = new BasicMappedWebpage
            {
                PublishOn = DateTime.Now.AddDays(-1),
                Name = "Test 1",
                Site = CurrentSite,
                Published = true
            };
            Session.Transact(session => session.Save(textPage));

            var errorPageOptions = _sut.GetErrorPageOptions(textPage.Id);

            errorPageOptions.Should().HaveCount(1);
            errorPageOptions[0].Selected.Should().BeTrue();
        }

        [Fact]
        public void SiteSettingsOptionGenerator_GetLayoutOptions_WithoutDefaultIncludedIsEmpty()
        {
            var errorPageOptions = _sut.GetLayoutOptions(-1);

            errorPageOptions.Should().HaveCount(0);
        }

        [Fact]
        public void SiteSettingsOptionGenerator_GetLayoutOptions_IncludesLayouts()
        {
            var layout = new Layout { Name = "Test Layout", Site = CurrentSite };
            Session.Transact(session => session.Save(layout));

            var errorPageOptions = _sut.GetLayoutOptions(-1);

            errorPageOptions.Should().HaveCount(1);
            errorPageOptions[0].Text.Should().Be("Test Layout");
            errorPageOptions[0].Value.Should().Be(layout.Id.ToString());
        }

        [Fact]
        public void SiteSettingsOptionGenerator_GetLayoutOptions_IfLayoutIdIsPassedFlagIsTrue()
        {
            var layout = new Layout { Name = "Test Layout", Site = CurrentSite };
            var layout2 = new Layout { Name = "Test Layout 2", Site = CurrentSite };
            Session.Transact(session =>
            {
                session.Save(layout);
                session.Save(layout2);
            });

            var errorPageOptions = _sut.GetLayoutOptions(layout2.Id);

            errorPageOptions.Should().HaveCount(2);
            errorPageOptions[0].Selected.Should().BeFalse();
            errorPageOptions[1].Selected.Should().BeTrue();
        }
    }
}