using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IGetAllAdminDescriptors
    {
        List<ControllerActionDescriptor> GetDescriptors();
    }
}