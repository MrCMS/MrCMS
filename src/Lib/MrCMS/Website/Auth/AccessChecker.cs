using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using MrCMS.ACL;
using MrCMS.Entities.People;

namespace MrCMS.Website.Auth
{
    public class AccessChecker : IAccessChecker
    {
        private readonly ICheckStandardAccessLogic _checkStandardAccessLogic;
        private readonly IPerformACLCheck _performAclCheck;

        public AccessChecker(ICheckStandardAccessLogic checkStandardAccessLogic,
            IPerformACLCheck performAclCheck)
        {
            _checkStandardAccessLogic = checkStandardAccessLogic;
            _performAclCheck = performAclCheck;
        }

        public async Task<bool> CanAccess<TAclRule>(string operation, ClaimsPrincipal claimsPrincipal) where TAclRule : ACLRule
        {
            return await CanAccess(typeof(TAclRule), operation, claimsPrincipal);
        }

        public async Task<bool> CanAccess(Type type, string operation, ClaimsPrincipal claimsPrincipal)
        {
            if (type == null || string.IsNullOrWhiteSpace(operation))
                return true;

            var result = _checkStandardAccessLogic.Check(claimsPrincipal);
            return  result.CanAccess ?? await _performAclCheck.CanAccessLogic(result.Roles, type, operation);
        }
    }
}