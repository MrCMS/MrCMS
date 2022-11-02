using Microsoft.AspNetCore.Mvc.Controllers;
using MrCMS.ACL;
using System.Collections.Generic;
using System.Reflection;

namespace MrCMS.Website.Auth
{
    public class GetAclKeys : IGetAclKeys
    {
        private readonly IAclKeyGenerator _aclKeyGenerator;

        public GetAclKeys(IAclKeyGenerator aclKeyGenerator)
        {
            _aclKeyGenerator = aclKeyGenerator;
        }

        public string GetKey<TAclRule>(string operation) where TAclRule : ACLRule
        {
            return _aclKeyGenerator.GetKey(AclType.ExplicitRule, typeof(TAclRule).Name, operation);
        }

        public IReadOnlyList<string> GetKeys<TAclRule>(string operation) where TAclRule : ACLRule
        {
            return new List<string>
            {
                GetKey<TAclRule>(operation)
            };
        }

        public IReadOnlyList<string> GetKeys(ControllerActionDescriptor descriptor)
        {
            var acl = descriptor.MethodInfo.GetCustomAttribute<AclAttribute>() ??
                      descriptor.ControllerTypeInfo.GetCustomAttribute<AclAttribute>();


            var keys = new List<string>
            {
                _aclKeyGenerator.GetKey(AclType.Controller, descriptor.ControllerName, null),
                _aclKeyGenerator.GetKey(AclType.Action, descriptor.ControllerName, descriptor.ActionName)
            };
            if (acl != null)
            {
                keys.Add(_aclKeyGenerator.GetKey(AclType.ExplicitRule, acl.Name, acl.Operation));
            }

            return keys;
        }
    }
}