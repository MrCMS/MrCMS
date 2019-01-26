using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Controllers;
using MrCMS.ACL;
using MrCMS.Entities.People;

namespace MrCMS.Website.Auth
{
    public interface IAccessChecker
    {
        bool CanAccess([AspMvcController]string controllerName, [AspMvcAction]string actionName);
        bool CanAccess([AspMvcController]string controllerName, [AspMvcAction]string actionName, User user);
        bool CanAccess(ControllerActionDescriptor descriptor);
        bool CanAccess(ControllerActionDescriptor descriptor, User user);
        bool CanAccess<TAclRule>(string operation) where TAclRule : ACLRule;
        bool CanAccess<TAclRule>(string operation, User user) where TAclRule : ACLRule;
    }
}