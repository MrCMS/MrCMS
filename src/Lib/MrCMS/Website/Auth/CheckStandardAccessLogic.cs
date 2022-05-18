using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.ACL;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Website.Auth
{
    public class CheckStandardAccessLogic : ICheckStandardAccessLogic
    {
        private readonly IUserRoleManager _userRoleManager;
        private readonly IConfigurationProvider _siteConfigurationProvider;

        private readonly IDictionary<int, StandardLogicCheckResult> _cachedResults =
            new ConcurrentDictionary<int, StandardLogicCheckResult>();

        public CheckStandardAccessLogic(IUserRoleManager userRoleManager,
            IConfigurationProvider siteConfigurationProvider)
        {
            _userRoleManager = userRoleManager;
            _siteConfigurationProvider = siteConfigurationProvider;
        }


        public async Task<StandardLogicCheckResult> Check(User user)
        {
            // must be logged in
            if (user == null)
                return new StandardLogicCheckResult { CanAccess = false };

            if (_cachedResults.ContainsKey(user.Id))
                return _cachedResults[user.Id];
            var result = await GetResult(user);
            _cachedResults[user.Id] = result;
            return result;
        }

        private async Task<StandardLogicCheckResult> GetResult(User user)
        {
            // if they're an admin they're always allowed
            if (await _userRoleManager.IsInRoleAsync(user, UserRole.Administrator))
                return new StandardLogicCheckResult { CanAccess = true };

            // if ACL isn't on, they're not allowed because they're not an admin
            var aclSettings = _siteConfigurationProvider.GetSiteSettings<ACLSettings>();
            if (!aclSettings.ACLEnabled)
                return new StandardLogicCheckResult { CanAccess = false };

            // if the user has no roles, they cannot have any acl access granted
            var roles = await _userRoleManager.GetRolesAsync(user);
            if (!roles.Any())
                return new StandardLogicCheckResult { CanAccess = false };

            return new StandardLogicCheckResult { Roles = roles };
        }
    }
}