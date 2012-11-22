using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Settings;
using Ninject.Activation;
using Ninject.Extensions.Conventions;
using Ninject.Extensions.Conventions.BindingGenerators;
using Ninject.Modules;
using Ninject.Syntax;
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
                                      .Where(t => !typeof(ISettings).IsAssignableFrom(t) && !typeof(IController).IsAssignableFrom(t))
                                      .BindWith<NinjectServiceToInterfaceBinder>()
                                      .Configure(onSyntax => onSyntax.InRequestScope()));
            Kernel.Bind(syntax => syntax.FromAssembliesMatching("MrCMS.*").SelectAllClasses()
                                      .Where(t => typeof(ISettings).IsAssignableFrom(t) && !typeof(IController).IsAssignableFrom(t))
                                      .BindWith<NinjectSettingsBinder>()
                                      .Configure(onSyntax => onSyntax.InRequestScope()));
        }
    }

    public class NinjectSettingsBinder : IBindingGenerator
    {
        public IEnumerable<IBindingWhenInNamedWithOrOnSyntax<object>> CreateBindings(Type type, IBindingRoot bindingRoot)
        {
            var list = new List<IBindingWhenInNamedWithOrOnSyntax<object>>();
            if (type.IsInterface || type.IsAbstract)
            {
                return list;
            }

            bindingRoot.Bind(type).ToMethod(context => GetValue(type, context));

            return list;
        }

        private static object GetValue(Type type, IContext context)
        {
            var configProvider =
                context.Kernel.Get(
                    typeof (ConfigurationProvider<>).MakeGenericType(type));
            var settingsObjectProperty =
                configProvider.GetType().GetProperties().FirstOrDefault(
                    info => info.Name == "Settings");

            if (settingsObjectProperty != null)
                return settingsObjectProperty.GetValue(configProvider, null);
            return null;
        }
    }
}