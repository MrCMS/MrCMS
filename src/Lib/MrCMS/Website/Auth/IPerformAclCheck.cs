using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using MrCMS.ACL;

namespace MrCMS.Website.Auth
{
    public interface IPerformACLCheck
    {
        Task<bool> CanAccessLogic(ISet<int> roles, Type aclType, string operation);
    }
}