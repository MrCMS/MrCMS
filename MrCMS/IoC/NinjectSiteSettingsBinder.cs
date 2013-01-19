using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Multisite;
using MrCMS.Services;
using MrCMS.Settings;
using Ninject;
using Ninject.Activation;
using Ninject.Extensions.Conventions.BindingGenerators;
using Ninject.Syntax;
using MrCMS.Helpers;

namespace MrCMS.IoC
{
    public class NinjectSiteSettingsBinder : IBindingGenerator
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
                context.Kernel.Get<ConfigurationProvider>();
            var method =
                typeof(ConfigurationProvider).GetMethodExt("GetSiteSettings");

            return method != null
                       ? method.MakeGenericMethod(type)
                               .Invoke(configProvider,
                                       new object[] { })
                       : null;
        }
    }
}