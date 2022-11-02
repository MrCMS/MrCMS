using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using MrCMS.ACL;
using MrCMS.Entities.People;

namespace MrCMS.Website.Auth
{
    public class AccessChecker : IAccessChecker
    {
        private readonly ICheckStandardAccessLogic _checkStandardAccessLogic;
        private readonly IPerformAclCheck _performAclCheck;

        public AccessChecker(ICheckStandardAccessLogic checkStandardAccessLogic,
            IPerformAclCheck performAclCheck)
        {
            _checkStandardAccessLogic = checkStandardAccessLogic;
            _performAclCheck = performAclCheck;
        }

        public async Task<bool> CanAccess(ControllerActionDescriptor descriptor, User user)
        {
            var result = await _checkStandardAccessLogic.Check(user);
            return result.CanAccess ?? _performAclCheck.CanAccessLogic(result.Roles, descriptor);
        }

        public async Task<bool> CanAccess<TAclRule>(string operation, User user) where TAclRule : ACLRule
        {
            var result = await _checkStandardAccessLogic.Check(user);
            return result.CanAccess ?? _performAclCheck.CanAccessLogic<TAclRule>(result.Roles, operation);
        }
    }
}