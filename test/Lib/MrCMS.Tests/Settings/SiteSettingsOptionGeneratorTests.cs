using FluentAssertions;
using MrCMS.Apps;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Tests.Stubs;
using System;
using AutoFixture.Xunit2;
using FakeItEasy;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.TestSupport;
using Xunit;

namespace MrCMS.Tests.Settings
{
    public class SiteSettingsOptionGeneratorTests : MrCMSTest
    {
        //private readonly MrCMSAppContext _mrCMSAppContext = new MrCMSAppContext();
        //private readonly SiteSettingsOptionGenerator _sut;

        //public SiteSettingsOptionGeneratorTests()
        //{
        //    _sut = new SiteSettingsOptionGenerator(Context, _mrCMSAppContext);
        //}
        [Theory, AutoFakeItEasyData]
        public void SiteSettingsOptionGenerator_GetErrorPageOptions_IsEmptyWithNoPages
            (SiteSettingsOptionGenerator sut)
        {

            var errorPageOptions = sut.GetErrorPageOptions(-1);

            errorPageOptions.Should().BeEmpty();
        }

        [Theory, AutoFakeItEasyData]
        public void SiteSettingsOptionGenerator_GetErrorPageOptions_IncludesSavedPublishedWebpages
            ([Frozen] IDataReader dataReader, SiteSettingsOptionGenerator sut)
        {
            var textPage = new BasicMappedWebpage
            {
                PublishOn = DateTime.Now.AddDays(-1),
                Name = "Test 1",
                Published = true
            };
            A.CallTo(() => dataReader.Readonly<Webpage>()).ReturnsAsAsyncQueryable(textPage);

            var errorPageOptions = sut.GetErrorPageOptions(-1);

            errorPageOptions.Should().HaveCount(1);
            errorPageOptions[0].Text.Should().Be("Test 1");
            errorPageOptions[0].Value.Should().Be(textPage.Id.ToString());
        }
        [Theory, AutoFakeItEasyData]
        public void SiteSettingOptionGenerator_GetErrorPageOptions_ExcludesSavedUnpublishedWebpages
            ([Frozen] IDataReader dataReader, SiteSettingsOptionGenerator sut)
        {
            var textPage = new BasicMappedWebpage { Name = "Test 1", Published = false };
            A.CallTo(() => dataReader.Readonly<Webpage>()).ReturnsAsAsyncQueryable(textPage);

            var errorPageOptions = sut.GetErrorPageOptions(-1);

            errorPageOptions.Should().HaveCount(0);
        }

        [Theory, AutoFakeItEasyData]
        public void SiteSettingsOptionGenerator_GetErrorPageOptions_ItemIsSelectedIfTheIdMatches
            ([Frozen] IDataReader dataReader, SiteSettingsOptionGenerator sut)
        {
            var textPage = new BasicMappedWebpage
            {
                Name = "Test 1",
                Published = true
            };
            A.CallTo(() => dataReader.Readonly<Webpage>()).ReturnsAsAsyncQueryable(textPage);

            var errorPageOptions = sut.GetErrorPageOptions(textPage.Id);

            errorPageOptions.Should().HaveCount(1);
            errorPageOptions[0].Selected.Should().BeTrue();
        }

        [Theory, AutoFakeItEasyData]
        public void SiteSettingsOptionGenerator_GetLayoutOptions_WithoutDefaultIncludedIsEmpty
            ([Frozen] IDataReader dataReader, SiteSettingsOptionGenerator sut)
        {
            var errorPageOptions = sut.GetLayoutOptions(-1);

            errorPageOptions.Should().HaveCount(0);
        }

        [Theory, AutoFakeItEasyData]
        public void SiteSettingsOptionGenerator_GetLayoutOptions_IncludesLayouts
            ([Frozen] IDataReader dataReader, SiteSettingsOptionGenerator sut)
        {
            var layout = new Layout { Name = "Test Layout", Id = 123 };
            A.CallTo(() => dataReader.Readonly<Layout>()).ReturnsAsAsyncQueryable(layout);

            var errorPageOptions = sut.GetLayoutOptions(-1);

            errorPageOptions.Should().HaveCount(1);
            errorPageOptions[0].Text.Should().Be("Test Layout");
            errorPageOptions[0].Value.Should().Be(layout.Id.ToString());
        }

        [Theory, AutoFakeItEasyData]
        public void SiteSettingsOptionGenerator_GetLayoutOptions_IfLayoutIdIsPassedFlagIsTrue
            ([Frozen] IDataReader dataReader, SiteSettingsOptionGenerator sut)
        {
            var layout = new Layout { Name = "Test Layout", Id = 123 };
            var layout2 = new Layout { Name = "Test Layout 2", Id = 234 };
            A.CallTo(() => dataReader.Readonly<Layout>()).ReturnsAsAsyncQueryable(layout, layout2);

            var errorPageOptions = sut.GetLayoutOptions(layout2.Id);

            errorPageOptions.Should().HaveCount(2);
            errorPageOptions[0].Selected.Should().BeFalse();
            errorPageOptions[1].Selected.Should().BeTrue();
        }
    }
}