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
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using Ninject.Web.Common;
using Ninject;

namespace MrCMS.IoC
{
    //Wires up IOC automatically
    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(syntax => syntax.From(TypeHelper.GetAllMrCMSAssemblies()).SelectAllClasses()
                .Where(
                    t =>
                        !typeof (SiteSettingsBase).IsAssignableFrom(t) &&
                        !typeof (IController).IsAssignableFrom(t) && !Kernel.GetBindings(t).Any())
                .BindWith<NinjectServiceToInterfaceBinder>()
                .Configure(onSyntax => onSyntax.InRequestScope()));
            Kernel.Bind(syntax => syntax.From(TypeHelper.GetAllMrCMSAssemblies()).SelectAllClasses()
                .Where(
                    t =>
                        typeof (SiteSettingsBase).IsAssignableFrom(t) && !typeof (IController).IsAssignableFrom(t) &&
                        !Kernel.GetBindings(t).Any())
                .BindWith<NinjectSiteSettingsBinder>()
                .Configure(onSyntax => onSyntax.InRequestScope()));

            Kernel.Bind<HttpRequestBase>().ToMethod(context => CurrentRequestData.CurrentContext.Request);
            Kernel.Bind<HttpResponseBase>().ToMethod(context => CurrentRequestData.CurrentContext.Response);
            Kernel.Bind<HttpSessionStateBase>().ToMethod(context => CurrentRequestData.CurrentContext.Session);
            Kernel.Bind<HttpServerUtilityBase>().ToMethod(context => CurrentRequestData.CurrentContext.Server);
            Kernel.Bind<ObjectCache>().ToMethod(context => MemoryCache.Default);
            Kernel.Bind<IAuthenticationManager>()
                  .ToMethod(context => context.Kernel.Get<HttpContextBase>().GetOwinContext().Authentication);
            Kernel.Bind<Cache>().ToMethod(context => CurrentRequestData.CurrentContext.Cache);
            Kernel.Bind(typeof(ITokenProvider<>)).To(typeof(PropertyTokenProvider<>)).InRequestScope();
            Kernel.Bind(typeof(IMessageParser<,>)).To(typeof(MessageParser<,>)).InRequestScope();
            Kernel.Rebind<Site>()
                  .ToMethod(context => CurrentRequestData.CurrentSite)
                  .InRequestScope();
            Kernel.Bind<IEnumerable<IHashAlgorithm>>()
                  .ToMethod(context => context.Kernel.GetAll<IHashAlgorithm>())
                  .InRequestScope();

            // Allowing IFileSystem implementation to be set in the site settings
            Kernel.Rebind<IFileSystem>().ToMethod(context =>
                                                      {
                                                          var storageType = context.Kernel.Get<FileSystemSettings>().StorageType;
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
            Kernel.Bind(typeof(ISearcher<,>)).To(typeof(FSDirectorySearcher<,>)).When(request => !UseAzureForLucene()).InRequestScope();
            Kernel.Bind(typeof(ISearcher<,>)).To(typeof(AzureDirectorySearcher<,>)).When(request => UseAzureForLucene()).InRequestScope();
            Kernel.Bind(typeof(IIndexManager<,>)).To(typeof(FSDirectoryIndexManager<,>)).When(request => !UseAzureForLucene()).InRequestScope();
            Kernel.Bind(typeof(IIndexManager<,>)).To(typeof(AzureDirectoryIndexManager<,>)).When(request => UseAzureForLucene()).InRequestScope();
        }

        public bool UseAzureForLucene()
        {
            return (Kernel.Get<IFileSystem>() is IAzureFileSystem) && Kernel.Get<FileSystemSettings>().UseAzureForLucene;
        }
    }
}