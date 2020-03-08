using Microsoft.AspNetCore.Mvc.Filters;
using MrCMS.DbConfiguration;

namespace MrCMS.Website.Filters
{
    internal class InstallationAwareResultFilter<T> : IResultFilter where T : IResultFilter
    {
        private readonly ICheckInstallationStatus _checkInstallationStatus;
        private IResultFilter _filter;

        public InstallationAwareResultFilter(ICheckInstallationStatus checkInstallationStatus)
        {
            _checkInstallationStatus = checkInstallationStatus;
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (!_checkInstallationStatus.IsInstalled())
            {
                return;
            }
            _filter = context.HttpContext.RequestServices.GetService(typeof(T)) as IResultFilter;
            _filter?.OnResultExecuting(context);
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            _filter?.OnResultExecuted(context);
        }
    }
}