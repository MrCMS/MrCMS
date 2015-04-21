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
        private Site _site;
        private readonly IConfigurationProvider _configurationProvider;

        public InitializeDatabase(ISession session, Site site, IConfigurationProvider configurationProvider)
        {
            _session = session;
            _site = site;
            _configurationProvider = configurationProvider;
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
                FormRendererType = FormRenderingType.Bootstrap3
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
            var mailSettings = new MailSettings
            {
                Port = 25,
            };
            var fileSystemSettings = new FileSystemSettings
            {
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
                Type = typeof(DeleteExpiredLogsTask).FullName,
                EveryXSeconds = 600
            };

            var deleteQueuedTask = new ScheduledTask
            {
                Type = typeof(DeleteOldQueuedTasks).FullName,
                EveryXSeconds = 600
            };

            var sendQueueEmailsTask = new ScheduledTask
            {
                Type = typeof(SendQueuedMessagesTask).FullName,
                EveryXSeconds = 30
            };

            var publishPagesTask = new ScheduledTask
            {
                Type = typeof(PublishScheduledWebpagesTask).FullName,
                EveryXSeconds = 10
            };

            var deleteOldLogsTask = new ScheduledTask
            {
                Type = typeof(DeleteExpiredLogsTask).FullName,
                EveryXSeconds = 600
            };
 

            _session.Transact(s =>
            {
                s.Save(deleteLogsTask);
                s.Save(deleteQueuedTask);
                s.Save(sendQueueEmailsTask);
                s.Save(publishPagesTask);
                s.Save(deleteOldLogsTask);
            });

        }
    }
}