using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using FluentAssertions;
using Microsoft.AspNet.Identity;
using MrCMS.DbConfiguration;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Indexing.Querying;
using MrCMS.IoC;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
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
            _kernel = new StandardKernel(new TestServiceModule(), new TestContextModule(),
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

    public class TestServiceModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(syntax => syntax.From(TypeHelper.GetAllMrCMSAssemblies()).SelectAllClasses()
                   .Where(
                       t =>
                           !typeof(SiteSettingsBase).IsAssignableFrom(t) &&
                           !typeof(IController).IsAssignableFrom(t) && !Kernel.GetBindings(t).Any())
                   .BindWith<NinjectServiceToInterfaceBinder>()
                   .Configure(onSyntax => onSyntax.InThreadScope()));
            Kernel.Bind(syntax => syntax.From(TypeHelper.GetAllMrCMSAssemblies()).SelectAllClasses()
                .Where(
                    t =>
                        typeof(SiteSettingsBase).IsAssignableFrom(t) && !typeof(IController).IsAssignableFrom(t) &&
                        !Kernel.GetBindings(t).Any())
                .BindWith<NinjectSiteSettingsBinder>()
                .Configure(onSyntax => onSyntax.InThreadScope()));

            Kernel.Bind<HttpRequestBase>().ToMethod(context => CurrentRequestData.CurrentContext.Request);
            Kernel.Bind<HttpResponseBase>().ToMethod(context => CurrentRequestData.CurrentContext.Response);
            Kernel.Bind<HttpSessionStateBase>().ToMethod(context => CurrentRequestData.CurrentContext.Session);
            Kernel.Bind<HttpServerUtilityBase>().ToMethod(context => CurrentRequestData.CurrentContext.Server);
            Kernel.Bind<Cache>().ToMethod(context => CurrentRequestData.CurrentContext.Cache);
            Kernel.Bind(typeof(ITokenProvider<>)).To(typeof(PropertyTokenProvider<>)).InThreadScope();
            Kernel.Bind(typeof(IMessageParser<,>)).To(typeof(MessageParser<,>)).InThreadScope();
            Kernel.Rebind<Site>()
                  .ToMethod(context => CurrentRequestData.CurrentSite)
                  .InThreadScope();
            Kernel.Bind<IEnumerable<IHashAlgorithm>>()
                  .ToMethod(context => context.Kernel.GetAll<IHashAlgorithm>())
                  .InThreadScope();

            // Allowing IFileSystem implementation to be set in the site settings
            Kernel.Rebind<IFileSystem>().ToMethod(context =>
            {
                var storageType = context.Kernel.Get<FileSystemSettings>().StorageType;
                if (!string.IsNullOrWhiteSpace(storageType))
                    return context.Kernel.Get(TypeHelper.GetTypeByName(storageType)) as IFileSystem;
                return context.Kernel.Get<FileSystem>();
            }).InThreadScope();
            Kernel.Rebind<IEnumerable<IFileSystem>>().ToMethod(context => TypeHelper
                                                                              .GetAllTypesAssignableFrom<IFileSystem>()
                                                                              .Select(
                                                                                  type =>
                                                                                  context.Kernel.Get(type) as
                                                                                  IFileSystem)).InThreadScope();
            Kernel.Bind<IUserStore<User>>().To<UserStore>().InThreadScope();
            Kernel.Bind<UserManager<User>>().ToMethod(context =>
            {
                var userManager = new UserManager<User>(context.Kernel.Get<IUserStore<User>>());
                userManager.UserValidator = new UserValidator<User>(userManager)
                {
                    AllowOnlyAlphanumericUserNames = false
                };
                return userManager;
            }).InThreadScope();
            Kernel.Bind(typeof(ISearcher<,>)).To(typeof(FSDirectorySearcher<,>)).When(request => !UseAzureForLucene()).InThreadScope();
            Kernel.Bind(typeof(ISearcher<,>)).To(typeof(AzureDirectorySearcher<,>)).When(request => UseAzureForLucene()).InThreadScope();
            Kernel.Bind(typeof(IIndexManager<,>)).To(typeof(FSDirectoryIndexManager<,>)).When(request => !UseAzureForLucene()).InThreadScope();
            Kernel.Bind(typeof(IIndexManager<,>)).To(typeof(AzureDirectoryIndexManager<,>)).When(request => UseAzureForLucene()).InThreadScope();
        }
        public bool UseAzureForLucene()
        {
            return (Kernel.Get<IFileSystem>() is IAzureFileSystem) && Kernel.Get<FileSystemSettings>().UseAzureForLucene;
        }
    }
}