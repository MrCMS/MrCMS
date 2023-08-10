using System.Reflection;
using System.Threading.Tasks;
using MrCMS.Services;
using MrCMS.Web.Admin.Infrastructure.Routing;
using MrCMS.Website;
using MrCMS.Website.Auth;

namespace MrCMS.Web.Admin.Infrastructure.Breadcrumbs
{
    public class BreadcrumbAccessChecker : IBreadcrumbAccessChecker
    {
        private readonly IGetAdminActionDescriptor _getAdminActionDescriptor;
        private readonly IAccessChecker _accessChecker;
        private readonly IGetCurrentClaimsPrincipal _getCurrentClaimsPrincipal;

        public BreadcrumbAccessChecker(IGetAdminActionDescriptor getAdminActionDescriptor, IAccessChecker accessChecker,
            IGetCurrentClaimsPrincipal getCurrentClaimsPrincipal)
        {
            _getAdminActionDescriptor = getAdminActionDescriptor;
            _accessChecker = accessChecker;
            _getCurrentClaimsPrincipal = getCurrentClaimsPrincipal;
        }

        public async Task<bool> CanAccess(Breadcrumb breadcrumb)
        {
            if (breadcrumb == null)
                return false;
            var descriptor = _getAdminActionDescriptor.GetDescriptor(breadcrumb.Controller, breadcrumb.Action);
            if (descriptor == null)
                return false;

            // check the method for an ACL attribute
            // if there is no ACL attribute, check the controller for an ACL attribute
            var aclAttribute = descriptor.MethodInfo.GetCustomAttribute<AclAttribute>() ??
                               descriptor.ControllerTypeInfo.GetCustomAttribute<AclAttribute>();

            // if there is no ACL attribute, then we can assume that the user has access
            // todo - check this
            if (aclAttribute == null)
                return false;

            // otherwise, check the ACL using info from the attribute
            var user = await _getCurrentClaimsPrincipal.GetPrincipal();
            return await _accessChecker.CanAccess(aclAttribute.Type, aclAttribute.Operation, user);

            // var user = await _getCurrentClaimsPrincipal.GetPrincipal();
            // return await _accessChecker.CanAccess(descriptor, user);
        }
    }
}