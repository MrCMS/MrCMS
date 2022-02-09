using FluentAssertions;
using MrCMS.Apps;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Tests.Stubs;
using System;
using System.Threading.Tasks;
using MrCMS.TestSupport;
using Xunit;

namespace MrCMS.Tests.Settings
{
    public class SiteSettingsOptionGeneratorTests : InMemoryDatabaseTest
    {
        private readonly MrCMSAppContext _mrCMSAppContext = new MrCMSAppContext();
        private readonly SiteSettingsOptionGenerator _sut;

        public SiteSettingsOptionGeneratorTests()
        {
            _sut = new SiteSettingsOptionGenerator(Session, _mrCMSAppContext);
        }
        [Fact]
        public void SiteSettingsOptionGenerator_GetErrorPageOptions_IsEmptyWithNoPages()
        {
            var errorPageOptions = _sut.GetTopLevelPageOptions(-1);

            errorPageOptions.Should().BeEmpty();
        }

        [Fact]
        public async Task SiteSettingsOptionGenerator_GetErrorPageOptions_IncludesSavedPublishedWebpages()
        {
            var textPage = new BasicMappedWebpage
            {
                PublishOn = DateTime.Now.AddDays(-1),
                Name = "Test 1",
                Site = CurrentSite,
                Published = true
            };
            await Session.TransactAsync(session => session.SaveAsync(textPage));

            var errorPageOptions = _sut.GetTopLevelPageOptions(-1);

            errorPageOptions.Should().HaveCount(1);
            errorPageOptions[0].Text.Should().Be("Test 1");
            errorPageOptions[0].Value.Should().Be(textPage.Id.ToString());
        }
        [Fact]
        public async Task SiteSettingOptionGenerator_GetErrorPageOptions_ExcludesSavedUnpublishedWebpages()
        {
            var textPage = new BasicMappedWebpage { Name = "Test 1" };
            await Session.TransactAsync(session => session.SaveAsync(textPage));

            var errorPageOptions = _sut.GetTopLevelPageOptions(-1);

            errorPageOptions.Should().HaveCount(0);
        }

        [Fact]
        public async Task SiteSettingsOptionGenerator_GetErrorPageOptions_ItemIsSelectedIfTheIdMatches()
        {
            var textPage = new BasicMappedWebpage
            {
                PublishOn = DateTime.Now.AddDays(-1),
                Name = "Test 1",
                Site = CurrentSite,
                Published = true
            };
            await Session.TransactAsync(session => session.SaveAsync(textPage));

            var errorPageOptions = _sut.GetTopLevelPageOptions(textPage.Id);

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
        public async Task SiteSettingsOptionGenerator_GetLayoutOptions_IncludesLayouts()
        {
            var layout = new Layout { Name = "Test Layout", Site = CurrentSite };
            await Session.TransactAsync(session => session.SaveAsync(layout));

            var errorPageOptions = _sut.GetLayoutOptions(-1);

            errorPageOptions.Should().HaveCount(1);
            errorPageOptions[0].Text.Should().Be("Test Layout");
            errorPageOptions[0].Value.Should().Be(layout.Id.ToString());
        }

        [Fact]
        public async Task SiteSettingsOptionGenerator_GetLayoutOptions_IfLayoutIdIsPassedFlagIsTrue()
        {
            var layout = new Layout { Name = "Test Layout", Site = CurrentSite };
            var layout2 = new Layout { Name = "Test Layout 2", Site = CurrentSite };
            await Session.TransactAsync(async session =>
            {
                await session.SaveAsync(layout);
                await session.SaveAsync(layout2);
            });

            var errorPageOptions = _sut.GetLayoutOptions(layout2.Id);

            errorPageOptions.Should().HaveCount(2);
            errorPageOptions[0].Selected.Should().BeFalse();
            errorPageOptions[1].Selected.Should().BeTrue();
        }
    }
}