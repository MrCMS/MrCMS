using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;
using MrCMS.Helpers;

namespace MrCMS.Services
{
    public class UserUIPermissionsService : IUserUIPermissionsService
    {
        private readonly IGetCurrentUser _getCurrentUser;
        private readonly IUserRoleManager _userRoleManager;
        private readonly IRepository<Webpage> _repository;

        public UserUIPermissionsService(IGetCurrentUser getCurrentUser, IUserRoleManager userRoleManager, IRepository<Webpage> repository)
        {
            _getCurrentUser = getCurrentUser;
            _userRoleManager = userRoleManager;
            _repository = repository;
        }

        public async Task<bool> IsCurrentUserAllowed(Webpage webpage)
        {
            User user = _getCurrentUser.Get();
            if (user != null && await _userRoleManager.IsAdmin(user)) return true;
            if (webpage.InheritFrontEndRolesFromParent)
            {
                var parent = webpage.ParentId.HasValue ? await _repository.Load(webpage.ParentId.Value) : null;
                if (parent != null)
                    return await IsCurrentUserAllowed(parent);
                return true;
            }
            if (!webpage.FrontEndAllowedRoles.Any()) return true;
            if (webpage.FrontEndAllowedRoles.Any() && user == null) return false;
            return user != null && user.UserToRoles.Select(y => y.UserRoleId)
                       .Intersect(webpage.FrontEndAllowedRoles.Select(y => y.RoleId)).Any();
        }
    }
}