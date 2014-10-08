using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Settings;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using Ninject.Web.Common;

namespace MrCMS.IoC.Modules
{
    //Wires up IOC automatically

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
}