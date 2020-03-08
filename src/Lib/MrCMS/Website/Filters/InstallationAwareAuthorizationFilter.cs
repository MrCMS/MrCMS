using Microsoft.AspNetCore.Mvc.Filters;
using MrCMS.DbConfiguration;

namespace MrCMS.Website.Filters
{
    internal class InstallationAwareAuthorizationFilter<T> : IAuthorizationFilter where T : IAuthorizationFilter
    {
        private readonly ICheckInstallationStatus _checkInstallationStatus;
        private IAuthorizationFilter _filter;

        public InstallationAwareAuthorizationFilter(ICheckInstallationStatus checkInstallationStatus)
        {
            _checkInstallationStatus = checkInstallationStatus;
        }


        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!_checkInstallationStatus.IsInstalled())
            {
                return;
            }
            _filter = context.HttpContext.RequestServices.GetService(typeof(T)) as IAuthorizationFilter;
            _filter?.OnAuthorization(context);
        }
    }
}