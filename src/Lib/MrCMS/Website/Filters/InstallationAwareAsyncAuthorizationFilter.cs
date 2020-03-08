using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using MrCMS.DbConfiguration;

namespace MrCMS.Website.Filters
{
    internal class InstallationAwareAsyncAuthorizationFilter<T> : IAsyncAuthorizationFilter where T : IAsyncAuthorizationFilter
    {
        private readonly ICheckInstallationStatus _checkInstallationStatus;

        public InstallationAwareAsyncAuthorizationFilter(ICheckInstallationStatus checkInstallationStatus)
        {
            _checkInstallationStatus = checkInstallationStatus;
        }

        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!_checkInstallationStatus.IsInstalled())
            {
                return Task.CompletedTask;
            }
            IAsyncAuthorizationFilter filter = context.HttpContext.RequestServices.GetService(typeof(T)) as IAsyncAuthorizationFilter;
            return filter.OnAuthorizationAsync(context);
        }
    }
}