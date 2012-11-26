using System.Web;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Settings;
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
                                      .Where(t => !typeof(ISettings).IsAssignableFrom(t) && !typeof(IController).IsAssignableFrom(t))
                                      .BindWith<NinjectServiceToInterfaceBinder>()
                                      .Configure(onSyntax => onSyntax.InRequestScope()));
            Kernel.Bind(syntax => syntax.FromAssembliesMatching("MrCMS.*").SelectAllClasses()
                                      .Where(t => typeof(ISettings).IsAssignableFrom(t) && !typeof(IController).IsAssignableFrom(t))
                                      .BindWith<NinjectSettingsBinder>()
                                      .Configure(onSyntax => onSyntax.InRequestScope()));

            Kernel.Bind<HttpRequestBase>().ToMethod(context => new HttpRequestWrapper(HttpContext.Current.Request));
        }
    }
}