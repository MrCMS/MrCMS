using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Controllers;
using MrCMS.ACL;
using MrCMS.Entities.People;

namespace MrCMS.Website.Auth
{
    public class PerformAclCheck : IPerformAclCheck
    {
        private readonly IGetAclKeys _getAclKeys;
        private readonly IGetAclRoles _getAclRoles;

        public PerformAclCheck(IGetAclKeys getAclKeys, IGetAclRoles getAclRoles)
        {
            _getAclKeys = getAclKeys;
            _getAclRoles = getAclRoles;
        }


        public bool CanAccessLogic(IEnumerable<string> roles, ControllerActionDescriptor descriptor)
        {
            var roleArray = roles as string[] ?? roles.ToArray();

            // administrator always has access
            if (roleArray.Contains(UserRole.Administrator))
                return true;

            var keys = _getAclKeys.GetKeys(descriptor);
            return CanAccessLogicInternal(roleArray, keys);
        }

        public bool CanAccessLogic<TAclRule>(IEnumerable<string> roles, string operation) where TAclRule : ACLRule
        {
            var roleArray = roles as string[] ?? roles.ToArray();

            // administrator always has access
            if (roleArray.Contains(UserRole.Administrator))
                return true;

            var keys = _getAclKeys.GetKeys<TAclRule>(operation);
            return CanAccessLogicInternal(roleArray, keys);
        }

        private bool CanAccessLogicInternal(IEnumerable<string> roles, IEnumerable<string> keys)
        {
            return _getAclRoles.GetRoles(roles, keys).Any();
        }
    }
}