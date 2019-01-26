using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs
{
    public class GetBreadcrumbTypes : IGetBreadcrumbTypes
    {
        private readonly IServiceProvider _serviceProvider;

        public GetBreadcrumbTypes(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        private Type GetParent(Type type)
        {
            var result = TypeHelper.TypeIsImplementationOfOpenGeneric(type, typeof(Breadcrumb<>));
            return result.IsImplementationOf
                ? result.MatchedType.GenericTypeArguments[0]
                : null;
        }

        public List<Type> GetHierarchy(Type type)
        {
            return TypeIterator(type).ToList();
        }

        public Type FindBreadcrumbType(string controllerName, string actionName)
        {
            var types = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(Breadcrumb));

            return types.FirstOrDefault(type =>
            {
                var breadcrumb = (_serviceProvider.GetService(type) as Breadcrumb);

                return breadcrumb.Controller?.Equals(controllerName, StringComparison.InvariantCultureIgnoreCase) ==
                       true
                       && breadcrumb.Action?.Equals(actionName, StringComparison.InvariantCultureIgnoreCase) == true;
            });
        }

        private IEnumerable<Type> TypeIterator(Type type)
        {
            yield return type;

            var parent = GetParent(type);
            while (parent != null)
            {
                yield return parent;
                parent = GetParent(parent);
            }
        }
    }
}