using Microsoft.AspNetCore.Mvc.Filters;
using MrCMS.DbConfiguration;

namespace MrCMS.Website.Filters
{
    internal class InstallationAwareResourceFilter<T> : IResourceFilter where T : IResourceFilter
    {
        private readonly ICheckInstallationStatus _checkInstallationStatus;
        private IResourceFilter _filter;

        public InstallationAwareResourceFilter(ICheckInstallationStatus checkInstallationStatus)
        {
            _checkInstallationStatus = checkInstallationStatus;
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (!_checkInstallationStatus.IsInstalled())
            {
                return;
            }
            _filter = context.HttpContext.RequestServices.GetService(typeof(T)) as IResourceFilter;
            _filter?.OnResourceExecuting(context);
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            _filter?.OnResourceExecuted(context);
        }
    }
}