using Microsoft.AspNetCore.Mvc.Filters;
using MrCMS.DbConfiguration;

namespace MrCMS.Website.Filters
{
    internal class InstallationAwareActionFilter<T> : IActionFilter where T : IActionFilter
    {
        private readonly ICheckInstallationStatus _checkInstallationStatus;
        private IActionFilter _filter;

        public InstallationAwareActionFilter(ICheckInstallationStatus checkInstallationStatus)
        {
            _checkInstallationStatus = checkInstallationStatus;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!_checkInstallationStatus.IsInstalled())
            {
                return;
            }
            _filter = context.HttpContext.RequestServices.GetService(typeof(T)) as IActionFilter;
            _filter?.OnActionExecuting(context);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _filter?.OnActionExecuted(context);
        }
    }
}