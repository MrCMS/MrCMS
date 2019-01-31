using Microsoft.Extensions.DependencyInjection;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Helpers
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