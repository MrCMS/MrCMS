using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using MrCMS.DbConfiguration;

namespace MrCMS.Website.Filters
{
    internal class InstallationAwareAsyncActionFilter<T> : IAsyncActionFilter where T : IAsyncActionFilter
    {
        private readonly ICheckInstallationStatus _checkInstallationStatus;

        public InstallationAwareAsyncActionFilter(ICheckInstallationStatus checkInstallationStatus)
        {
            _checkInstallationStatus = checkInstallationStatus;
        }
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!_checkInstallationStatus.IsInstalled())
            {
                return next();
            }
            IAsyncActionFilter filter = context.HttpContext.RequestServices.GetService(typeof(T)) as IAsyncActionFilter;
            return filter.OnActionExecutionAsync(context, next);
        }
    }
}