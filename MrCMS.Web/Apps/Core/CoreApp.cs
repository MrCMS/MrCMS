using System;
using System.Collections.Generic;
using MrCMS.Apps;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Installation;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;
using Ninject;

namespace MrCMS.Web.Apps.Core
{
    public class CoreApp : MrCMSApp
    {
        public override string AppName
        {
            get { return "Core"; }
        }

        protected override int InstallOrder
        {
            get { return 1; }
        }

        protected override void RegisterServices(IKernel kernel)
        {
            
        }

        protected override void OnInstallation(ISession session, InstallModel model, Site site)
        {
            //settings
            var mediaSettings = new MediaSettings();
            session.Transact(sess => sess.Save(site));
            CurrentRequestData.CurrentSite = site;
            var currentSite = new CurrentSite(site);

            var siteSettings = new SiteSettings
            {
                Site = site,
                TimeZone = model.TimeZone,
                UICulture = model.UiCulture
            };
            CurrentRequestData.SiteSettings = siteSettings;

            var documentService = new DocumentService(session, siteSettings, currentSite);
            var layoutAreaService = new LayoutAreaService(session);

            var user = new User
            {
                Email = model.AdminEmail,
                IsActive = true
            };

            var authorisationService = new AuthorisationService();
            authorisationService.ValidatePassword(model.AdminPassword, model.ConfirmPassword);
            authorisationService.SetPassword(user, model.AdminPassword, model.ConfirmPassword);
            session.Transact(sess => sess.Save(user));
            CurrentRequestData.CurrentUser = user;

            documentService.AddDocument(model.BaseLayout);
            var layoutAreas = new List<LayoutArea>
                                  {
                                      new LayoutArea
                                          {
                                              AreaName = "Main Navigation",
                                              CreatedOn = CurrentRequestData.Now,
                                              Layout = model.BaseLayout,
                                              Site = site
                                          },
                                      new LayoutArea
                                          {
                                              AreaName = "Before Content",
                                              CreatedOn = CurrentRequestData.Now,
                                              Layout = model.BaseLayout,
                                              Site = site
                                          },
                                      new LayoutArea
                                          {
                                              AreaName = "After Content",
                                              CreatedOn = CurrentRequestData.Now,
                                              Layout = model.BaseLayout,
                                              Site = site
                                          }
                                  };

            foreach (LayoutArea l in layoutAreas)
                layoutAreaService.SaveArea(l);

            documentService.AddDocument(model.HomePage);
            documentService.AddDocument(model.Page2);
            documentService.AddDocument(model.Page3);
            documentService.AddDocument(model.Error403);
            documentService.AddDocument(model.Error404);
            documentService.AddDocument(model.Error500);
            var webpages = session.QueryOver<Webpage>().List();
            webpages.ForEach(documentService.PublishNow);


            var defaultMediaCategory = new MediaCategory
            {
                Name = "Default",
                UrlSegment = "default",
                Site = site
            };
            documentService.AddDocument(defaultMediaCategory);

            siteSettings.DefaultLayoutId = model.BaseLayout.Id;
            siteSettings.Error403PageId = model.Error403.Id;
            siteSettings.Error404PageId = model.Error404.Id;
            siteSettings.Error500PageId = model.Error500.Id;

            siteSettings.EnableInlineEditing = true;
            siteSettings.SiteIsLive = true;

            mediaSettings.ThumbnailImageHeight = 50;
            mediaSettings.ThumbnailImageWidth = 50;
            mediaSettings.LargeImageHeight = 800;
            mediaSettings.LargeImageWidth = 800;
            mediaSettings.MediumImageHeight = 500;
            mediaSettings.MediumImageWidth = 500;
            mediaSettings.SmallImageHeight = 200;
            mediaSettings.SmallImageWidth = 200;
            mediaSettings.ResizeQuality = 90;

            var configurationProvider = new ConfigurationProvider(new SettingService(session),
                                                                  currentSite);
            var fileSystemSettings = new FileSystemSettings { StorageType = typeof(FileSystem).FullName };
            configurationProvider.SaveSettings(siteSettings);
            configurationProvider.SaveSettings(mediaSettings);
            configurationProvider.SaveSettings(fileSystemSettings);


            var adminUserRole = new UserRole
            {
                Name = UserRole.Administrator
            };

            user.Roles = new List<UserRole> { adminUserRole };
            adminUserRole.Users = new List<User> { user };
            var roleService = new RoleService(session);
            roleService.SaveRole(adminUserRole);

            user.Sites = new List<Site> { site };
            site.Users = new List<User> { user };

            authorisationService.Logout();
            authorisationService.SetAuthCookie(user.Email, false);

            //set up system tasks
            //var taskService = MrCMSApplication.Get<IScheduledTaskManager>();
            //taskService.Add(new ScheduledTask
            //                    {
            //                        Type = "MrCMS.Tasks.SendQueuedMessagesTask",
            //                        EveryXMinutes = 1,
            //                        Site = site
            //                    });

            var service = new IndexService(session, currentSite);
            IndexService.GetIndexManagerOverride = (indexType, indexDefinitionInterface) => Activator.CreateInstance(
                typeof(FSDirectoryIndexManager<,>).MakeGenericType(
                    indexDefinitionInterface.GetGenericArguments()[0],
                    indexType), currentSite) as IIndexManagerBase;
            var mrCMSIndices = service.GetIndexes();
            mrCMSIndices.ForEach(index => service.Reindex(index.TypeName));
            mrCMSIndices.ForEach(index => service.Optimise(index.TypeName));
            IndexService.GetIndexManagerOverride = null;
        }

        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
        }
    }
}