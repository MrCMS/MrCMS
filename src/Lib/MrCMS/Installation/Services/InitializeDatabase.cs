using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Multisite;
using MrCMS.Installation.Models;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Tasks;

namespace MrCMS.Installation.Services
{
    public class InitializeDatabase : IInitializeDatabase
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly ISystemConfigurationProvider _systemConfigurationProvider;
        private readonly ITaskSettingManager _taskSettingManager;

        public InitializeDatabase(IConfigurationProvider configurationProvider, ISystemConfigurationProvider systemConfigurationProvider, ITaskSettingManager taskSettingManager)
        {
            _configurationProvider = configurationProvider;
            _systemConfigurationProvider = systemConfigurationProvider;
            _taskSettingManager = taskSettingManager;
        }

        public async Task Initialize(InstallModel model)
        {
            await SetupTasks();
            var siteSettings = new SiteSettings
            {
                TimeZone = model.TimeZone,
                UICulture = model.UiCulture,
                EnableInlineEditing = true,
                SiteIsLive = true,
                FormRendererType = FormRenderingType.Bootstrap4,

            };
            var mediaSettings = new MediaSettings
            {
                ThumbnailImageHeight = 50,
                ThumbnailImageWidth = 50,
                LargeImageHeight = 800,
                LargeImageWidth = 800,
                MediumImageHeight = 500,
                MediumImageWidth = 500,
                SmallImageHeight = 200,
                SmallImageWidth = 200,
                ResizeQuality = 90,
            };

            var fileSystemSettings = new FileSystemSettings
            {
                StorageType = typeof(FileSystem).FullName
            };
            var mailSettings = new MailSettings { SystemEmailAddress = model.AdminEmail };

            await _configurationProvider.SaveSettings(siteSettings);
            await _configurationProvider.SaveSettings(mediaSettings);
            await _configurationProvider.SaveSettings(fileSystemSettings);
            await _systemConfigurationProvider.SaveSettings(mailSettings);

        }
        private async Task SetupTasks()
        {
            await _taskSettingManager.Update(typeof(DeleteExpiredLogsTask), true, 600);
            //_taskSettingManager.Update(typeof(DeleteOldQueuedTasks), true, 600);
            await _taskSettingManager.Update(typeof(SendQueuedMessagesTask), false, 30);
            await _taskSettingManager.Update(typeof(PublishScheduledWebpagesTask), true, 10);
            await _taskSettingManager.Update(typeof(UpdateSitemap), true, 600);
        }
    }
}