using FakeItEasy;
using FluentAssertions;
using MrCMS.Settings;
using MrCMS.Web.Apps.Commenting.Settings;
using Xunit;

namespace MrCMS.Commenting.Tests.Services.CommentingSettingsAdminServiceTests
{
    public class GetSettings
    {
        [Fact]
        public void ReturnsConfigProviderGetSettings()
        {
            var configurationProvider = A.Fake<IConfigurationProvider>();
            var settings = new CommentingSettings();
            A.CallTo(() => configurationProvider.GetSiteSettings<CommentingSettings>()).Returns(settings);
            var service = new CommentingSettingsAdminServiceBuilder().WithConfigurationProvider(configurationProvider).Build();

            var commentingSettings = service.GetSettings();

            commentingSettings.Should().Be(settings);
        }
    }
}