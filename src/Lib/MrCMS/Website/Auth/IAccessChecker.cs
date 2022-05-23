using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using MrCMS.ACL;
using MrCMS.Entities.People;

namespace MrCMS.Website.Auth
{
    public interface IAccessChecker
    {
        Task<bool> CanAccess(ControllerActionDescriptor descriptor, User user);
        Task<bool> CanAccess<TAclRule>(string operation, User user) where TAclRule : ACLRule;
    }
}