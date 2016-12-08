using System;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Indexing;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Tasks;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Installation
{
    public class InitializeDatabase : IInitializeDatabase
    {
        private readonly ISession _session;
        private Site _site;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly ITaskSettingManager _taskSettingManager;

        public InitializeDatabase(ISession session, Site site, IConfigurationProvider configurationProvider, ITaskSettingManager taskSettingManager)
        {
            _session = session;
            _site = site;
            _configurationProvider = configurationProvider;
            _taskSettingManager = taskSettingManager;
        }

        public void Initialize(InstallModel model)
        {
            CurrentRequestData.CurrentSite = _site = _session.Get<Site>(_site.Id);
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

            _configurationProvider.SaveSettings(siteSettings);
            _configurationProvider.SaveSettings(mediaSettings);
            _configurationProvider.SaveSettings(fileSystemSettings);

        }
        private void SetupTasks()
        {
            _taskSettingManager.Update(typeof (DeleteExpiredLogsTask), true, 600);
            _taskSettingManager.Update(typeof (DeleteOldQueuedTasks), true, 600);
            _taskSettingManager.Update(typeof(SendQueuedMessagesTask), false, 30);
            _taskSettingManager.Update(typeof (PublishScheduledWebpagesTask), true, 10);
            _taskSettingManager.Update(typeof (OptimiseIndexes), true, 600);
            _taskSettingManager.Update(typeof (UpdateSitemap), true, 600);
        }
    }
}