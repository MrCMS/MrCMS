using FakeItEasy;
using MrCMS.Settings;
using MrCMS.Web.Apps.Commenting.Services;

namespace MrCMS.Commenting.Tests.Services.CommentingSettingsAdminServiceTests
{
    public class CommentingSettingsAdminServiceBuilder
    {
        private IConfigurationProvider _configurationProvider = A.Fake<IConfigurationProvider>();
        private IProcessCommentWidgetChanges _processCommentWidgetChanges = A.Fake<IProcessCommentWidgetChanges>();

        public CommentingSettingsAdminServiceBuilder WithConfigurationProvider(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
            return this;
        }
        public CommentingSettingsAdminServiceBuilder WithWidgetChangeProcessor(IProcessCommentWidgetChanges processCommentWidgetChanges)
        {
            _processCommentWidgetChanges = processCommentWidgetChanges;
            return this;
        }

        public CommentingSettingsAdminService Build()
        {
            return new CommentingSettingsAdminService(_configurationProvider, _processCommentWidgetChanges);
        }
    }
}