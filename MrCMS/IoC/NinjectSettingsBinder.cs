using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Settings;
using Ninject;
using Ninject.Activation;
using Ninject.Extensions.Conventions.BindingGenerators;
using Ninject.Syntax;

namespace MrCMS.IoC
{
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