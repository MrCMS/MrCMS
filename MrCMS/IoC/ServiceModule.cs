using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Indexing.Querying;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Extensions.Conventions.Syntax;
using Ninject.Modules;
using Ninject.Web.Common;

namespace MrCMS.IoC
{
    //Wires up IOC automatically
    public class SettingsModule : NinjectModule
    {
        private readonly bool _forTest;

        public SettingsModule(bool forTest = false)
        {
            _forTest = forTest;
        }

        public override void Load()
        {
            Kernel.Bind(syntax =>
            {
                IJoinExcludeIncludeBindSyntax joinExcludeIncludeBindSyntax = syntax.From(
                    TypeHelper.GetAllMrCMSAssemblies()).SelectAllClasses()
                    .Where(
                        t =>
                            typeof(SiteSettingsBase).IsAssignableFrom(t) && !Kernel.GetBindings(t).Any());

                if (_forTest)
                {
                    joinExcludeIncludeBindSyntax.BindToSelf();
                }
                else
                {
                    joinExcludeIncludeBindSyntax
                        .BindWith<NinjectSiteSettingsBinder>()
                        .Configure(onSyntax => onSyntax.InRequestScope());
                }
            });
            Kernel.Bind(syntax =>
            {
                IJoinExcludeIncludeBindSyntax joinExcludeIncludeBindSyntax = syntax.From(
                    TypeHelper.GetAllMrCMSAssemblies()).SelectAllClasses()
                    .Where(
                        t =>
                            typeof(SystemSettingsBase).IsAssignableFrom(t) && !Kernel.GetBindings(t).Any());

                if (_forTest)
                {
                    joinExcludeIncludeBindSyntax.BindToSelf();
                }
                else
                {
                    joinExcludeIncludeBindSyntax
                        .BindWith<NinjectSystemSettingsBinder>()
                        .Configure(onSyntax => onSyntax.InRequestScope());
                }
            });
        }
    }

    public class SystemWebModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<HttpRequestBase>().ToMethod(context => CurrentRequestData.CurrentContext.Request);
            Kernel.Bind<HttpResponseBase>().ToMethod(context => CurrentRequestData.CurrentContext.Response);
            Kernel.Bind<HttpSessionStateBase>().ToMethod(context => CurrentRequestData.CurrentContext.Session);
            Kernel.Bind<HttpServerUtilityBase>().ToMethod(context => CurrentRequestData.CurrentContext.Server);
            Kernel.Bind<ObjectCache>().ToMethod(context => MemoryCache.Default);
            Kernel.Bind<Cache>().ToMethod(context => CurrentRequestData.CurrentContext.Cache);
            Kernel.Bind<UrlHelper>()
                .ToMethod(context => new UrlHelper(CurrentRequestData.CurrentContext.Request.RequestContext));
        }
    }

    public class FileSystemModule : NinjectModule
    {
        public override void Load()
        {
            // Allowing IFileSystem implementation to be set in the site settings
            Kernel.Rebind<IFileSystem>().ToMethod(context =>
            {
                string storageType = context.Kernel.Get<FileSystemSettings>().StorageType;
                if (!string.IsNullOrWhiteSpace(storageType))
                    return context.Kernel.Get(TypeHelper.GetTypeByName(storageType)) as IFileSystem;
                return context.Kernel.Get<FileSystem>();
            }).InRequestScope();
            Kernel.Rebind<IEnumerable<IFileSystem>>().ToMethod(context => TypeHelper
                .GetAllTypesAssignableFrom<IFileSystem>()
                .Select(
                    type =>
                        context.Kernel.Get(type) as
                            IFileSystem)).InRequestScope();

        }
    }

    public class GenericBindingsModule:NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(typeof(ITokenProvider<>)).To(typeof(PropertyTokenProvider<>)).InRequestScope();
            Kernel.Bind(typeof(IMessageParser<,>)).To(typeof(MessageParser<,>)).InRequestScope();
        }
    }

    public class SiteModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Rebind<Site>()
                .ToMethod(context => CurrentRequestData.CurrentSite)
                .InRequestScope();
        }
    }

    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(syntax => syntax.From(TypeHelper.GetAllMrCMSAssemblies()).SelectAllClasses()
                .Where(
                    t =>
                        !typeof(SiteSettingsBase).IsAssignableFrom(t) &&
                        !typeof(SystemSettingsBase).IsAssignableFrom(t) &&
                        !typeof(IController).IsAssignableFrom(t) && !Kernel.GetBindings(t).Any())
                .BindWith<NinjectServiceToInterfaceBinder>()
                .Configure(onSyntax => onSyntax.InRequestScope()));


        }
    }

    public class LuceneModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(typeof(ISearcher<,>))
                .To(typeof(FSDirectorySearcher<,>))
                .When(request => !UseAzureForLucene())
                .InRequestScope();
            Kernel.Bind(typeof(ISearcher<,>))
                .To(typeof(AzureDirectorySearcher<,>))
                .When(request => UseAzureForLucene())
                .InRequestScope();
            Kernel.Bind(typeof(IIndexManager<,>))
                .To(typeof(FSDirectoryIndexManager<,>))
                .When(request => !UseAzureForLucene())
                .InRequestScope();
            Kernel.Bind(typeof(IIndexManager<,>))
                .To(typeof(AzureDirectoryIndexManager<,>))
                .When(request => UseAzureForLucene())
                .InRequestScope();
        }

        private bool UseAzureForLucene()
        {
            return (Kernel.Get<IFileSystem>() is IAzureFileSystem) && Kernel.Get<FileSystemSettings>().UseAzureForLucene;
        }
    }
    public class AuthModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IAuthenticationManager>().ToMethod(context => context.Kernel.Get<HttpContextBase>().GetOwinContext().Authentication);
            Kernel.Bind<IEnumerable<IHashAlgorithm>>()
                .ToMethod(context => context.Kernel.GetAll<IHashAlgorithm>())
                .InRequestScope();
            Kernel.Bind<IUserStore<User>>().To<UserStore>().InRequestScope();
            Kernel.Bind<UserManager<User>>().ToMethod(context =>
            {
                var userManager = new UserManager<User>(context.Kernel.Get<IUserStore<User>>());
                userManager.UserValidator = new UserValidator<User>(userManager)
                {
                    AllowOnlyAlphanumericUserNames = false
                };
                return userManager;
            }).InRequestScope();

        }
    }
}