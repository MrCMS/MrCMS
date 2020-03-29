using System.Threading.Tasks;
using MrCMS.Web.Admin.Infrastructure.Routing;
using MrCMS.Website.Auth;

namespace MrCMS.Web.Admin.Infrastructure.Breadcrumbs
{
    public class BreadcrumbAccessChecker : IBreadcrumbAccessChecker
    {
        private readonly IGetAdminActionDescriptor _getAdminActionDescriptor;
        private readonly IAccessChecker _accessChecker;

        public BreadcrumbAccessChecker(IGetAdminActionDescriptor getAdminActionDescriptor, IAccessChecker accessChecker)
        {
            _getAdminActionDescriptor = getAdminActionDescriptor;
            _accessChecker = accessChecker;
        }

        public async Task<bool> CanAccess(Breadcrumb breadcrumb)
        {
            if (breadcrumb == null)
                return false;
            var descriptor = _getAdminActionDescriptor.GetDescriptor(breadcrumb.Controller, breadcrumb.Action);
            if (descriptor == null)
                return false;

            return await _accessChecker.CanAccess(descriptor);
        }
    }
}