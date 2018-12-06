using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Events;
using MrCMS.Helpers;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System.Collections.Generic;
using System.Reflection;

namespace MrCMS.Tests
{
    public abstract class InMemoryDatabaseTest : MrCMSTest
    {
        private static Configuration Configuration;
        private static ISessionFactory SessionFactory;
        protected ISession Session;
        private readonly object lockObject = new object();

        protected InMemoryDatabaseTest()
        {
            if (Configuration == null)
            {
                lock (lockObject)
                {
                    var assemblies = new List<Assembly> { typeof(InMemoryDatabaseTest).Assembly };
                    var nHibernateModule = new NHibernateConfigurator(new SqliteInMemoryProvider(), new MrCMSAppContext())
                    {
                        CacheEnabled = true,
                        ManuallyAddedAssemblies = assemblies
                    };
                    Configuration = nHibernateModule.GetConfiguration();

                    SessionFactory = Configuration.BuildSessionFactory();
                }
            }
            var site = new Site { Name = "Current Site", BaseUrl = "www.currentsite.com", Id = 1 };
            ServiceCollection.AddTransient<Site>(provider => site);
            Session = SessionFactory.OpenFilteredSession(ServiceProvider);
            ServiceCollection.AddTransient<ISession>(provider => Session);
            ServiceCollection.AddTransient<IStatelessSession>(provider => SessionFactory.OpenStatelessSession(Session.Connection));

            new SchemaExport(Configuration).Execute(false, true, false, Session.Connection, null);

            SetupUser();


            CurrentSite = Session.Transact(session =>
            {
                ServiceCollection.AddTransient<Site>(provider => site);
                session.Save(site);
                return site;
            });

            //CurrentRequestData.SiteSettings = new SiteSettings { TimeZone = TimeZoneInfo.Local.Id };

            //CurrentRequestData.ErrorSignal = new ErrorSignal();

            //ServiceCollection.Unbind<IEventContext>();
            //ServiceCollection.Load(new ServiceModule());
            //ServiceCollection.Load(new SettingsModule(true));
            //ServiceCollection.Load(new FileSystemModule());
            //ServiceCollection.Load(new SiteModule());
            //ServiceCollection.Rebind<IExternalUserSource>().ToConstant(A.Fake<IExternalUserSource>());
            //_eventContext = new TestableEventContext(/*ServiceCollection.Get<EventContext>()*/);
            //ServiceCollection.Rebind<IEventContext>().ToMethod(context => EventContext);
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

            //CurrentRequestData.CurrentUser = user;
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
    }

    public class DummyEvent : IEvent
    {
    }
}