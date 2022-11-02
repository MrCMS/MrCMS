using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Controllers;
using MrCMS.ACL;

namespace MrCMS.Website.Auth
{
    public interface IPerformAclCheck
    {
        bool CanAccessLogic(IEnumerable<string> roles, ControllerActionDescriptor descriptor);
        bool CanAccessLogic<TAclRule>(IEnumerable<string> roles, string operation) where TAclRule : ACLRule;
    }
}