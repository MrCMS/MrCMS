using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Routing
{
    public interface IGetAllAdminDescriptors
    {
        List<ControllerActionDescriptor> GetDescriptors();
    }
}