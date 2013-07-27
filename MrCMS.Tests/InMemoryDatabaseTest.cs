using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Web;
using Elmah;
using FakeItEasy;
using MrCMS.DbConfiguration;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Tasks;
using MrCMS.Tests.Stubs;
using MrCMS.Website;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace MrCMS.Tests
{
    public abstract class MrCMSTest : IDisposable
    {
        private readonly ListDictionary _listDictionary;

        protected MrCMSTest()
        {
            var httpContextWrapper = A.Fake<HttpContextBase>();
            _listDictionary = new ListDictionary();
            A.CallTo(() => httpContextWrapper.Items).Returns(_listDictionary);
            CurrentRequestData.OverridenContext = httpContextWrapper;
            CurrentRequestData.SiteSettings = new SiteSettings();
        }

        public virtual void Dispose()
        {
            CurrentRequestData.OverridenContext = null;
            _listDictionary.Clear();
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

            new SchemaExport(Configuration).Execute(false, true, false, Session.Connection, null);

            SetupUser();

            SetupRootChildren();

            CurrentSite = Session.Transact(session =>
                {
                    var site = new Site { Name = "Current Site", BaseUrl = "www.currentsite.com" };
                    CurrentRequestData.CurrentSite = site;
                    session.SaveOrUpdate(site);
                    return site;
                });

            CurrentRequestData.SiteSettings = new SiteSettings {TimeZone = TimeZoneInfo.Local.Id};

            TaskExecutor.Discard();

            CurrentRequestData.ErrorSignal = new ErrorSignal();
        }

        protected Site CurrentSite { get; set; }


        private void SetupRootChildren()
        {
            MrCMSApplication.OverridenRootChildren = new List<Webpage>();
        }

        private void SetupUser()
        {
            var user = new User
                           {
                               Email = "test@example.com",
                               IsActive = true,
                           };

            new AuthorisationService().SetPassword(user, "password", "password");

            var adminUserRole = new UserRole
                                    {
                                        Name = UserRole.Administrator
                                    };

            user.Roles = new List<UserRole> { adminUserRole };
            adminUserRole.Users = new List<User> { user };

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