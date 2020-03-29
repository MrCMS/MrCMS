using Microsoft.Extensions.DependencyInjection;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Areas.Admin.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterBreadcrumbs(this IServiceCollection serviceCollection)
        {
            var breadcrumbTypes = TypeHelper.GetAllConcreteTypesAssignableFrom<Breadcrumb>();
            breadcrumbTypes.ForEach(type => serviceCollection.AddTransient(type));
            return serviceCollection;
        }
    }
}