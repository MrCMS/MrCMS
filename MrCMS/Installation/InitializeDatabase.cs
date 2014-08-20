using System;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
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
        private readonly Site _site;
        private readonly IConfigurationProvider _configurationProvider;

        public InitializeDatabase(ISession session, Site site, IConfigurationProvider configurationProvider)
        {
            _session = session;
            _site = site;
            _configurationProvider = configurationProvider;
        }

        public void Initialize(InstallModel model)
        {
            SetupTasks();
            var siteSettings = new SiteSettings
            {
                SiteId = _site.Id,
                TimeZone = model.TimeZone,
                UICulture = model.UiCulture,
                EnableInlineEditing = true,
                SiteIsLive = true,
                FormRendererType = FormRenderingType.Bootstrap3
            };
            var mediaSettings = new MediaSettings
            {
                SiteId = _site.Id,
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
            var mailSettings = new MailSettings
            {
                SiteId = _site.Id,
                Port = 25,
            };
            var fileSystemSettings = new FileSystemSettings
            {
                SiteId = _site.Id,
                StorageType = typeof (FileSystem).FullName
            };

            _configurationProvider.SaveSettings(siteSettings);
            _configurationProvider.SaveSettings(mediaSettings);
            _configurationProvider.SaveSettings(mailSettings);
            _configurationProvider.SaveSettings(fileSystemSettings);

        }
        private void SetupTasks()
        {
            var deleteLogsTask = new ScheduledTask
            {
                Site = _site,
                Type = typeof(DeleteExpiredLogsTask).FullName,
                EveryXSeconds = 60
            };

            var deleteQueuedTask = new ScheduledTask
            {
                Site = _site,
                Type = typeof(DeleteOldQueuedTasks).FullName,
                EveryXSeconds = 60
            };

            var sendQueueEmailsTask = new ScheduledTask
            {
                Site = _site,
                Type = typeof(SendQueuedMessagesTask).FullName,
                EveryXSeconds = 60
            };

            _session.Transact(s =>
            {
                s.Save(deleteLogsTask);
                s.Save(deleteQueuedTask);
                s.Save(sendQueueEmailsTask);
            });

        }
    }
}