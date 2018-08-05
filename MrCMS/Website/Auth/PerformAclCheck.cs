using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MrCMS.Website.Auth
{
    public class PerformAclCheck : IPerformAclCheck
    {
        private readonly IGetAclRoles _getAclRoles;

        public PerformAclCheck(IGetAclRoles getAclRoles)
        {
            _getAclRoles = getAclRoles;
        }

        public async Task<bool> CanAccessLogic(StandardLogicCheckResult result, IList<string> keys)
        {
            return result.CanAccess ?? (await _getAclRoles.GetRoles(result.Roles, keys)).Any();
        }
    }
}