using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using MrCMS.DbConfiguration;

namespace MrCMS.Website.Filters
{
    internal class InstallationAwareAsyncResourceFilter<T> : IAsyncResourceFilter where T : IAsyncResourceFilter
    {
        private readonly ICheckInstallationStatus _checkInstallationStatus;

        public InstallationAwareAsyncResourceFilter(ICheckInstallationStatus checkInstallationStatus)
        {
            _checkInstallationStatus = checkInstallationStatus;
        }
        public Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            if (!_checkInstallationStatus.IsInstalled())
            {
                return next();
            }
            IAsyncResourceFilter filter = context.HttpContext.RequestServices.GetService(typeof(T)) as IAsyncResourceFilter;
            return filter.OnResourceExecutionAsync(context, next);
        }
    }
}