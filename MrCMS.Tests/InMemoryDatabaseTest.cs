using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using Elmah;
using FakeItEasy;
using FakeItEasy.Core;
using Iesi.Collections.Generic;
using MrCMS.DbConfiguration;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.IoC;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Tests.Stubs;
using MrCMS.Website;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using Ninject;
using Ninject.MockingKernel;
using Ninject.Modules;
using Configuration = NHibernate.Cfg.Configuration;
using System.Linq;

namespace MrCMS.Tests
{
    public class TestContextModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<HttpContextBase>().To<OutOfContext>().InThreadScope();
        }
    }
    public abstract class MrCMSTest : IDisposable
    {
        private readonly MockingKernel _kernel;
        private readonly IEventContext _eventContext = A.Fake<IEventContext>();
        public IEnumerable<ICompletedFakeObjectCall> EventsRaised
        {
            get { return Fake.GetCalls(_eventContext); }
        }

        protected MrCMSTest()
        {
            _kernel = new MockingKernel();
            Kernel.Load(new TestContextModule());
            Kernel.Bind<IEventContext>().ToMethod(context => _eventContext);
            MrCMSApplication.OverrideKernel(Kernel);
            CurrentRequestData.SiteSettings = new SiteSettings();
        }

        public MockingKernel Kernel { get { return _kernel; } }

        public virtual void Dispose()
        {
        }
    }

    public abstract class InMemoryDatabaseTest : MrCMSTest
    {
        private static Configuration Configuration;
        private static ISessionFactory SessionFactory;
        protected static ISession Session;
        private readonly object lockObject = new object();

        protected InMemoryDatabaseTest()
        {
            if (Configuration == null)
            {
                lock (lockObject)
                {
                    var assemblies = new List<Assembly> { typeof(BasicMappedWebpage).Assembly };
                    var nHibernateModule = new NHibernateConfigurator
                                               {
                                                   CacheEnabled = true,
                                                   DatabaseType = DatabaseType.Sqlite,
                                                   InDevelopment = true,
                                                   ManuallyAddedAssemblies = assemblies
                                               };
                    Configuration = nHibernateModule.GetConfiguration();

                    SessionFactory = Configuration.BuildSessionFactory();
                }
            }
            Session = SessionFactory.OpenFilteredSession();
            Kernel.Bind<ISession>().ToMethod(context => Session);

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

            user.Roles = new HashedSet<UserRole> { adminUserRole };
            adminUserRole.Users = new HashedSet<User> { user };

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
    }
}