using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Services;
using System;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System.Collections.Generic;
using System.Reflection;

namespace MrCMS.TestSupport
{
    public abstract class InMemoryDatabaseTest : MrCMSTest
    {
        private static Configuration Configuration;
        private static ISessionFactory SessionFactory;
        protected ISession Session;
        private static readonly object LockObject = new object();

        protected InMemoryDatabaseTest()
        {
            lock (LockObject)
            {
                if (SessionFactory == null)
                {
                    var assemblies = new List<Assembly> {typeof(InMemoryDatabaseTest).Assembly};
                    var configurator = new NHibernateConfigurator(new SqliteInMemoryProvider(), new MrCMSAppContext())
                    {
                        ManuallyAddedAssemblies = assemblies
                    };
                    var configuration = configurator.GetConfiguration();

                    // only publish once fully built, so a failure is reported by every test rather than
                    // leaving a half-initialised factory behind
                    SessionFactory = configuration.BuildSessionFactory();
                    Configuration = configuration;
                }
            }

            // each test gets a fresh database, so nothing cached by a previous test may be reused
            ClearSecondLevelCache();

            var site = new Site {Name = "Current Site", BaseUrl = "www.currentsite.com", Id = 1};
            CurrentSite = site;
            // entities saved through the session need the current site (see SetCoreProperties)
            ServiceCollection.AddTransient<ICurrentSiteLocator>(_ => new FixedSiteLocator(() => CurrentSite));
            // resolve services lazily so registrations a test makes after construction are still seen
            Session = SessionFactory.OpenFilteredSession(new LazyServiceProvider(() => ServiceProvider));
            ServiceCollection.AddTransient<ISession>(provider => Session);
            ServiceCollection.AddTransient<IStatelessSession>(provider =>
                SessionFactory.OpenStatelessSession(Session.Connection));

            new SchemaExport(Configuration).Execute(false, true, false, Session.Connection, null);

            // tests assume the current site exists in the database (it is referenced by every site entity)
            using (var transaction = Session.BeginTransaction())
            {
                Session.SaveAsync(site).GetAwaiter().GetResult();
                transaction.CommitAsync().GetAwaiter().GetResult();
            }

            SetupUser();
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

        private static void ClearSecondLevelCache()
        {
            SessionFactory.EvictQueries();
            foreach (var entityName in SessionFactory.GetAllClassMetadata().Keys)
                SessionFactory.EvictEntity(entityName);
            foreach (var role in SessionFactory.GetAllCollectionMetadata().Keys)
                SessionFactory.EvictCollection(role);
        }


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

            user.Roles = new HashSet<UserRole> {adminUserRole};
            adminUserRole.Users = new HashSet<User> {user};

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

    internal class LazyServiceProvider : IServiceProvider
    {
        private readonly Func<IServiceProvider> _getProvider;

        public LazyServiceProvider(Func<IServiceProvider> getProvider)
        {
            _getProvider = getProvider;
        }

        public object GetService(Type serviceType) => _getProvider().GetService(serviceType);
    }

    internal class FixedSiteLocator : ICurrentSiteLocator
    {
        private readonly Func<Site> _getSite;

        public FixedSiteLocator(Func<Site> getSite)
        {
            _getSite = getSite;
        }

        public Site GetCurrentSite() => _getSite();
        public RedirectedDomain GetCurrentRedirectedDomain() => null;
    }

    public class DummyEvent : IEvent
    {
    }
}