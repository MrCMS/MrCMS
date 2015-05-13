using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using Elmah;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.IoC.Modules;
using MrCMS.Messages;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using Ninject;

namespace MrCMS.Web.Tests
{
    public abstract class InMemoryDatabaseTest : MrCMSTest
    {
        private static Configuration Configuration;
        private static ISessionFactory SessionFactory;
        protected static ISession Session;
        private readonly IMessageTemplateProvider _messageTemplateProvider = new StubMessageTemplateProvider();
        private readonly object lockObject = new object();

        protected InMemoryDatabaseTest()
        {
            if (Configuration == null)
            {
                lock (lockObject)
                {
                    var assemblies = new List<Assembly> { typeof(InMemoryDatabaseTest).Assembly };
                    var nHibernateModule = new NHibernateConfigurator(new SqliteInMemoryProvider())
                    {
                        CacheEnabled = true,
                        ManuallyAddedAssemblies = assemblies
                    };
                    Configuration = nHibernateModule.GetConfiguration();

                    SessionFactory = Configuration.BuildSessionFactory();
                }
            }
            Session = SessionFactory.OpenFilteredSession(Kernel.Get<HttpContextBase>());
            Kernel.Bind<ISession>().ToMethod(context => Session);
            Kernel.Bind<IStatelessSession>().ToMethod(context => SessionFactory.OpenStatelessSession());

            new SchemaExport(Configuration).Execute(false, true, false, Session.Connection, null);

            SetupUser();


            CurrentSite = Session.Transact(session =>
            {
                var site = new Site { Name = "Current Site", BaseUrl = "www.currentsite.com", Id = 1 };
                CurrentRequestData.CurrentSite = site;
                session.Save(site);
                return site;
            });

            CurrentRequestData.SiteSettings = new SiteSettings { TimeZone = TimeZoneInfo.Local.Id };

            CurrentRequestData.ErrorSignal = new ErrorSignal();

            Kernel.Unbind<IEventContext>();
            Kernel.Load(new ServiceModule());
            Kernel.Load(new SettingsModule(true));
            Kernel.Load(new FileSystemModule());
            Kernel.Load(new SiteModule());
            Kernel.Load(new GenericBindingsModule());
            Kernel.Unbind<IMessageTemplateProvider>();
            Kernel.Bind<IMessageTemplateProvider>().ToConstant(_messageTemplateProvider);
            _eventContext = new TestableEventContext(Kernel.Get<EventContext>());
            Kernel.Rebind<IEventContext>().ToMethod(context => EventContext);
        }

        protected Site CurrentSite { get; set; }


        private void SetupUser()
        {
            var user = new User
            {
                Email = "test@example.com",
                IsActive = true,
            };

            var adminUserRole = new UserRole
            {
                Name = UserRole.Administrator
            };

            user.Roles = new HashSet<UserRole> { adminUserRole };
            adminUserRole.Users = new HashSet<User> { user };

            CurrentRequestData.CurrentUser = user;
        }

        public override void Dispose()
        {
            Dispose(true);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (Session != null)
                {
                    Session.Dispose();
                    Session = null;
                }
                base.Dispose();
            }
        }

        private class StubMessageTemplateProvider : IMessageTemplateProvider
        {
            public void SaveTemplate(MessageTemplate messageTemplate)
            {
            }

            public void SaveSiteOverride(MessageTemplate messageTemplate, Site site)
            {
            }

            public void DeleteSiteOverride(MessageTemplate messageTemplate, Site site)
            {
            }

            public List<MessageTemplate> GetAllMessageTemplates(Site site)
            {
                throw new NotImplementedException();
            }

            public T GetMessageTemplate<T>(Site site) where T : MessageTemplate, new()
            {
                return new T();
            }

            public MessageTemplate GetNewMessageTemplate(Type type)
            {
                throw new NotImplementedException();
            }
        }
    }
}