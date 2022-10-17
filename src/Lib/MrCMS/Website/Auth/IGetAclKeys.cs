using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Controllers;
using MrCMS.ACL;

namespace MrCMS.Website.Auth
{
    public interface IGetAclKeys
    {
        string GetKey<TAclRule>(string operation) where TAclRule : ACLRule;
        IReadOnlyList<string> GetKeys<TAclRule>(string operation) where TAclRule : ACLRule;
        IReadOnlyList<string> GetKeys(ControllerActionDescriptor descriptor);
    }
}