using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using MrCMS.ACL;
using MrCMS.Entities.People;

namespace MrCMS.Website.Auth
{
    public interface IAccessChecker
    {
        bool CanAccess(ControllerActionDescriptor descriptor);
        bool CanAccess(ControllerActionDescriptor descriptor, User user);
        bool CanAccess<TAclRule>(string operation) where TAclRule : ACLRule;
        bool CanAccess<TAclRule>(string operation, User user) where TAclRule : ACLRule;
    }
}