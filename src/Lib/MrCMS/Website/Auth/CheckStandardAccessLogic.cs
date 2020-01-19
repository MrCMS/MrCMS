using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.ACL;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Website.Auth
{
    public class CheckStandardAccessLogic : ICheckStandardAccessLogic
    {
        private readonly IGetCurrentUser _getCurrentUser;
        private readonly IUserRoleManager _userRoleManager;
        private readonly IConfigurationProvider _siteConfigurationProvider;

        private readonly IDictionary<int, StandardLogicCheckResult> _cachedResults =
            new ConcurrentDictionary<int, StandardLogicCheckResult>();

        public CheckStandardAccessLogic(IGetCurrentUser getCurrentUser, IUserRoleManager userRoleManager,
            IConfigurationProvider siteConfigurationProvider)
        {
            _getCurrentUser = getCurrentUser;
            _userRoleManager = userRoleManager;
            _siteConfigurationProvider = siteConfigurationProvider;
        }

        public StandardLogicCheckResult Check()
        {
            return Check(_getCurrentUser.Get());
        }

        public StandardLogicCheckResult Check(User user)
        {
            // must be logged in
            if (user == null)
                return new StandardLogicCheckResult { CanAccess = false };

            if (_cachedResults.ContainsKey(user.Id))
                return _cachedResults[user.Id];
            var result = GetResult(user);
            _cachedResults[user.Id] = result;
            return result;
        }

        private StandardLogicCheckResult GetResult(User user)
        {
            // if they're an admin they're always allowed
            if (_userRoleManager.IsInRoleAsync(user, Role.Administrator).ExecuteSync()) 
                return new StandardLogicCheckResult { CanAccess = true };

            // if ACL isn't on, they're not allowed because they're not an admin
            var aclSettings = _siteConfigurationProvider.GetSiteSettings<ACLSettings>();
            if (!aclSettings.ACLEnabled)
                return new StandardLogicCheckResult { CanAccess = false };

            // if the user has no roles, they cannot have any acl access granted
            var roles = _userRoleManager.GetRolesAsync(user).ExecuteSync(); 
            if (!roles.Any())
                return new StandardLogicCheckResult { CanAccess = false };

            return new StandardLogicCheckResult { Roles = roles };
        }
    }
}