using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using MrCMS.Entities.Multisite;
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
            Kernel.Bind(syntax => syntax.FromAssembliesMatching("MrCMS.*").SelectAllClasses()
                                      .Where(t => !typeof(SettingsBase).IsAssignableFrom(t) && !typeof(IController).IsAssignableFrom(t) && !Kernel.GetBindings(t).Any())
                                      .BindWith<NinjectServiceToInterfaceBinder>()
                                      .Configure(onSyntax => onSyntax.InRequestScope()));
            Kernel.Bind(syntax => syntax.FromAssembliesMatching("MrCMS.*").SelectAllClasses()
                                      .Where(t => typeof(SiteSettingsBase).IsAssignableFrom(t) && !typeof(IController).IsAssignableFrom(t) && !Kernel.GetBindings(t).Any())
                                      .BindWith<NinjectSiteSettingsBinder>()
                                      .Configure(onSyntax => onSyntax.InRequestScope()));
            Kernel.Bind(syntax => syntax.FromAssembliesMatching("MrCMS.*").SelectAllClasses()
                                      .Where(t => typeof(GlobalSettingsBase).IsAssignableFrom(t) && !typeof(IController).IsAssignableFrom(t) && !Kernel.GetBindings(t).Any())
                                      .BindWith<NinjectGlobalSettingsBinder>()
                                      .Configure(onSyntax => onSyntax.InRequestScope()));

            //Kernel.Bind<HttpContextBase>().ToMethod(context => MrCMSApplication.CurrentContext);
            Kernel.Bind<HttpRequestBase>().ToMethod(context => CurrentRequestData.CurrentContext.Request);
            Kernel.Bind<HttpSessionStateBase>().ToMethod(context => CurrentRequestData.CurrentContext.Session);
            Kernel.Bind<ObjectCache>().ToMethod(context => MemoryCache.Default);
            Kernel.Bind<Cache>().ToMethod(context => CurrentRequestData.CurrentContext.Cache);
            Kernel.Bind(typeof (ISearcher<,>)).To(typeof (FSDirectorySearcher<,>)).InRequestScope();
            Kernel.Rebind<CurrentSite>()
                  .ToMethod(context => new CurrentSite(context.Kernel.Get<ISiteService>().GetCurrentSite()))
                  .InRequestScope();
        }
    }
}