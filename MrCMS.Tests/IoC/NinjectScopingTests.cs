using System;
using FluentAssertions;
using MrCMS.DbConfiguration;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.IoC;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using Ninject;
using Xunit;

namespace MrCMS.Tests.IoC
{
    public class NinjectScopingTests :IDisposable
    {
        private readonly StandardKernel _kernel;

        private static Configuration Configuration;
        private static ISessionFactory SessionFactory;
        private static ISession Session;
        private readonly object lockObject = new object();
        public NinjectScopingTests()
        {
            var nHibernateConfigurator = new NHibernateConfigurator
            {
                CacheEnabled = true,
                DatabaseType = DatabaseType.Sqlite,
                InDevelopment = true,
            };
            Configuration = nHibernateConfigurator.GetConfiguration();

            SessionFactory = Configuration.BuildSessionFactory();

            Session = SessionFactory.OpenFilteredSession();

            new SchemaExport(Configuration).Execute(false, true, false, Session.Connection, null);
            _kernel = new StandardKernel(new ServiceModule(), new ContextModule(),
                                         new NHibernateModule(nHibernateConfigurator,
                                                              getSessionFactory: () => SessionFactory,
                                                              getSession: () => Session));
            MrCMSApplication.OverrideKernel(_kernel);
            Session.Transact(session => session.Save(new Site()));
        }

        [Fact]
        public void Kernel_Get_RequestingTheSameItemTwiceShouldGetTheSameInstanceTwice()
        {
            var documentService1 = _kernel.Get<IDocumentService>();
            var documentService2 = _kernel.Get<IDocumentService>();

            documentService1.Should().BeSameAs(documentService2);
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
                    SessionFactory.Dispose();
                    SessionFactory = null;
                    Configuration=null;
                }
            }
        }
    }
}