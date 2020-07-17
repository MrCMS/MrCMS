using System.Collections.Generic;
using System.Net.Http;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace MrCMS.Web.Admin.Infrastructure.Routing
{
    public interface IGetAllAdminDescriptors
    {
        List<ControllerActionDescriptor> GetDescriptors();

    }
}