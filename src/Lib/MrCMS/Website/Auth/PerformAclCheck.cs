using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using MrCMS.ACL;
using MrCMS.Entities.People;

namespace MrCMS.Website.Auth
{
    public class PerformAclCheck : IPerformACLCheck
    {
        private readonly IGetACLKeys _getAclKeys;
        private readonly IGetACLRoles _getAclRoles;

        public PerformAclCheck(IGetACLKeys getAclKeys, IGetACLRoles getAclRoles)
        {
            _getAclKeys = getAclKeys;
            _getAclRoles = getAclRoles;
        }


        public async Task<bool> CanAccessLogic(ISet<int> roles, Type aclType, string operation)
        {
            var keys = _getAclKeys.GetKeys(aclType, operation);
            return await CanAccessLogicInternal(roles, keys);
        }


        private async Task<bool> CanAccessLogicInternal(ISet<int> roles, IEnumerable<string> keys)
        {
            return await _getAclRoles.AnyRoles(roles, keys);
        }
    }
}