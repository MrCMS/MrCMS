using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using MrCMS.DbConfiguration;

namespace MrCMS.Website.Filters
{
    internal class InstallationAwareAsyncResultFilter<T> : IAsyncResultFilter where T : IAsyncResultFilter
    {
        private readonly ICheckInstallationStatus _checkInstallationStatus;

        public InstallationAwareAsyncResultFilter(ICheckInstallationStatus checkInstallationStatus)
        {
            _checkInstallationStatus = checkInstallationStatus;
        }
        public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (!_checkInstallationStatus.IsInstalled())
            {
                return next();
            }
            IAsyncResultFilter filter = context.HttpContext.RequestServices.GetService(typeof(T)) as IAsyncResultFilter;
            return filter.OnResultExecutionAsync(context, next);
        }
    }
}