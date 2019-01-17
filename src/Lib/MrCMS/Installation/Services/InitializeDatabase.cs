using MrCMS.Entities.Multisite;
using MrCMS.Installation.Models;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Tasks;
using NHibernate;

namespace MrCMS.Installation.Services
{
    public class InitializeDatabase : IInitializeDatabase
    {
        private readonly ISession _session;
        private Site _site;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly ISystemConfigurationProvider _systemConfigurationProvider;
        private readonly ITaskSettingManager _taskSettingManager;

        public InitializeDatabase(ISession session, Site site, IConfigurationProvider configurationProvider, ISystemConfigurationProvider systemConfigurationProvider, ITaskSettingManager taskSettingManager)
        {
            _session = session;
            _site = site;
            _configurationProvider = configurationProvider;
            _systemConfigurationProvider = systemConfigurationProvider;
            _taskSettingManager = taskSettingManager;
        }

        public void Initialize(InstallModel model)
        {
            SetupTasks();
            var siteSettings = new SiteSettings
            {
                TimeZone = model.TimeZone,
                UICulture = model.UiCulture,
                EnableInlineEditing = true,
                SiteIsLive = true,
                FormRendererType = FormRenderingType.Bootstrap3,

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
            var mailSettings = new MailSettings {SystemEmailAddress = model.AdminEmail};

            _configurationProvider.SaveSettings(siteSettings);
            _configurationProvider.SaveSettings(mediaSettings);
            _configurationProvider.SaveSettings(fileSystemSettings);
            _systemConfigurationProvider.SaveSettings(mailSettings);

        }
        private void SetupTasks()
        {
            _taskSettingManager.Update(typeof(DeleteExpiredLogsTask), true, 600);
            //_taskSettingManager.Update(typeof(DeleteOldQueuedTasks), true, 600);
            _taskSettingManager.Update(typeof(SendQueuedMessagesTask), false, 30);
            _taskSettingManager.Update(typeof(PublishScheduledWebpagesTask), true, 10);
            _taskSettingManager.Update(typeof(UpdateSitemap), true, 600);
        }
    }
}