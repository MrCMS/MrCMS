using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Admin.Infrastructure.Routing;
using MrCMS.Website.Auth;

namespace MrCMS.Web.Admin.Infrastructure.Breadcrumbs
{
    public class BreadcrumbAccessChecker : IBreadcrumbAccessChecker
    {
        private readonly IGetAdminActionDescriptor _getAdminActionDescriptor;
        private readonly IAccessChecker _accessChecker;
        private IGetCurrentUser _getCurrentUser;

        public BreadcrumbAccessChecker(IGetAdminActionDescriptor getAdminActionDescriptor, IAccessChecker accessChecker,
            IGetCurrentUser getCurrentUser)
        {
            _getAdminActionDescriptor = getAdminActionDescriptor;
            _accessChecker = accessChecker;
            _getCurrentUser = getCurrentUser;
        }

        public async Task<bool> CanAccess(Breadcrumb breadcrumb)
        {
            if (breadcrumb == null)
                return false;
            var descriptor = _getAdminActionDescriptor.GetDescriptor(breadcrumb.Controller, breadcrumb.Action);
            if (descriptor == null)
                return false;

            User user = await _getCurrentUser.Get();
            return await _accessChecker.CanAccess(descriptor, user);
        }
    }
}