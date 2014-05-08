using FakeItEasy;
using MrCMS.Settings;
using MrCMS.Web.Apps.Commenting.Services;
using MrCMS.Web.Apps.Commenting.Settings;
using Xunit;

namespace MrCMS.Commenting.Tests.Services.CommentingSettingsAdminServiceTests
{
    public class UpdateSettings
    {
        [Fact]
        public void CallsSaveSettingsOnTheNewSettings()
        {
            var configurationProvider = A.Fake<IConfigurationProvider>();
            var service = new CommentingSettingsAdminServiceBuilder().WithConfigurationProvider(configurationProvider).Build();
            var commentingSettings = new CommentingSettings();

            service.UpdateSettings(commentingSettings);

            A.CallTo(() => configurationProvider.SaveSettings(commentingSettings)).MustHaveHappened();
        }

        [Fact]
        public void CallsProcessCommentChanges()
        {
            var widgetChanges = A.Fake<IProcessCommentWidgetChanges>();
            var configurationProvider = A.Fake<IConfigurationProvider>();
            var existingSettings = new CommentingSettings();
            A.CallTo(() => configurationProvider.GetSiteSettings<CommentingSettings>()).Returns(existingSettings);
            var service = new CommentingSettingsAdminServiceBuilder().WithWidgetChangeProcessor(widgetChanges)
                                                             .WithConfigurationProvider(configurationProvider)
                                                             .Build();
            var newSettings = new CommentingSettings();

            service.UpdateSettings(newSettings);

            A.CallTo(() => widgetChanges.Process(existingSettings, newSettings)).MustHaveHappened();
        }
    }
}