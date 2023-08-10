using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using MrCMS.ACL;
using MrCMS.Entities.People;

namespace MrCMS.Website.Auth
{
    public interface IAccessChecker
    {
        Task<bool> CanAccess<TAclRule>(string operation, ClaimsPrincipal claimsPrincipal) where TAclRule : ACLRule;
        Task<bool> CanAccess(Type type, string operation, ClaimsPrincipal claimsPrincipal);

    }
}