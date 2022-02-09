using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;

namespace MrCMS.Web.Admin.Infrastructure.Breadcrumbs
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

        private static readonly HashSet<Type> BreadcrumbTypes =
            TypeHelper.GetAllConcreteTypesAssignableFrom<Breadcrumb>();

        public Type FindBreadcrumbType(string controllerName, string actionName)
        {
            return BreadcrumbTypes.FirstOrDefault(type =>
            {
                var breadcrumb = _serviceProvider.GetService(type) as Breadcrumb;

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