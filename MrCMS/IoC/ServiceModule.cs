using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Website;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using Ninject.Web.Common;

namespace MrCMS.IoC
{
    //Wires up IOC automatically
    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(syntax => syntax.FromAssembliesMatching("MrCMS.*").SelectAllClasses()
                                      .Where(t => !typeof(SettingsBase).IsAssignableFrom(t) && !typeof(IController).IsAssignableFrom(t))
                                      .BindWith<NinjectServiceToInterfaceBinder>()
                                      .Configure(onSyntax => onSyntax.InRequestScope()));
            Kernel.Bind(syntax => syntax.FromAssembliesMatching("MrCMS.*").SelectAllClasses()
                                      .Where(t => typeof(SiteSettingsBase).IsAssignableFrom(t) && !typeof(IController).IsAssignableFrom(t))
                                      .BindWith<NinjectSiteSettingsBinder>()
                                      .Configure(onSyntax => onSyntax.InRequestScope()));
            Kernel.Bind(syntax => syntax.FromAssembliesMatching("MrCMS.*").SelectAllClasses()
                                      .Where(t => typeof(GlobalSettingsBase).IsAssignableFrom(t) && !typeof(IController).IsAssignableFrom(t))
                                      .BindWith<NinjectGlobalSettingsBinder>()
                                      .Configure(onSyntax => onSyntax.InRequestScope()));

            //Kernel.Bind<HttpContextBase>().ToMethod(context => MrCMSApplication.CurrentContext);
            Kernel.Bind<HttpRequestBase>().ToMethod(context => MrCMSApplication.CurrentContext.Request);
            Kernel.Bind<HttpSessionStateBase>().ToMethod(context => MrCMSApplication.CurrentContext.Session);
            Kernel.Bind<ObjectCache>().ToMethod(context => MemoryCache.Default);
        }
    }
}