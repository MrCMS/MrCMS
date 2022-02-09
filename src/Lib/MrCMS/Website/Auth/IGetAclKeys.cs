using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Controllers;
using MrCMS.ACL;

namespace MrCMS.Website.Auth
{
    public interface IGetAclKeys
    {
        IList<string> GetKeys<TAclRule>(string operation) where TAclRule : ACLRule;
        IList<string> GetKeys(ControllerActionDescriptor descriptor);
    }
}