using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;

namespace MrCMS.Web.Admin.Infrastructure.Breadcrumbs
{
    public class GetNavigationTypes : IGetNavigationTypes
    {
        private static readonly HashSet<Type> BreadcrumbTypes =
            TypeHelper.GetAllConcreteTypesAssignableFrom<Breadcrumb>();

        public IEnumerable<Type> GetChildren(Type type)
        {
            return BreadcrumbTypes.FindAll(x => typeof(Breadcrumb<>).MakeGenericType(type).IsAssignableFrom(x));
        }

        public IEnumerable<Type> GetRootNavTypes()
        {
            // we only want direct implementations of Breadcrumb
            return BreadcrumbTypes.Where(x => x.BaseType == typeof(Breadcrumb)).ToHashSet();
        }
    }
}