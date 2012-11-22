using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using MrCMS.DbConfiguration;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace MrCMS.Tests
{
    public abstract class InMemoryDatabaseTest : IDisposable
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
                    var assemblies = new List<Assembly> {typeof (TextPage).Assembly};
                    var nHibernateModule = new NHibernateConfigurator
                                               {
                                                   CacheEnabled = true,
                                                   DatabaseType = DatabaseType.Sqlite,
                                                   InDevelopment = true,
                                                   ManuallyAddedAssemblies = assemblies
                                               };
                    Configuration= nHibernateModule.GetConfiguration();

                    SessionFactory = Configuration.BuildSessionFactory();
                }
            }

            MrCMSApplication.OverriddenSession = Session = SessionFactory.OpenSession();

            new SchemaExport(Configuration).Execute(false, true, false, Session.Connection, null);
        }

        public void Dispose()
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
            }
        }
    }
}