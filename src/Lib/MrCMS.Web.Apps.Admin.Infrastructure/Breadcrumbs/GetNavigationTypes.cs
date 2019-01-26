using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs
{
    public class GetNavigationTypes : IGetNavigationTypes
    {
        public IEnumerable<Type> GetChildren(Type type)
        {
            return TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(Breadcrumb<>).MakeGenericType(type));

        }

        public IEnumerable<Type> GetRootNavTypes()
        {
            var implementations = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(Breadcrumb));

            // we only want direct implementations of Breadcrumb
            return implementations.Where(x => !x.IsImplementationOf(typeof(Breadcrumb<>)));
        }
    }
}