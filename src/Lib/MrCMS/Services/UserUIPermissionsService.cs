using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public class UserUIPermissionsService : IUserUIPermissionsService
    {
        private readonly IGetCurrentUser _getCurrentUser;

        public UserUIPermissionsService(IGetCurrentUser getCurrentUser)
        {
            _getCurrentUser = getCurrentUser;
        }

        public async Task<PageAccessPermission> IsCurrentUserAllowed(Webpage webpage)
        {
            if (!webpage.HasCustomPermissions) return PageAccessPermission.Allowed;
            
            User user = await _getCurrentUser.Get();
            if (user != null && user.IsAdmin) return PageAccessPermission.Allowed;

            if (!webpage.FrontEndAllowedRoles.Any()) return PageAccessPermission.Allowed;
            if (webpage.FrontEndAllowedRoles.Any() && user == null) return PageAccessPermission.Unauthorized;
            return user.Roles.Intersect(webpage.FrontEndAllowedRoles).Any()
                ? PageAccessPermission.Allowed
                : PageAccessPermission.Forbidden;
        }
    }
}