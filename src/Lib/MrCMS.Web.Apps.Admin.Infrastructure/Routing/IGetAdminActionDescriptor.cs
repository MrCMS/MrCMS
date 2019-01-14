using Microsoft.AspNetCore.Mvc.Controllers;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Routing
{
    public interface IGetAdminActionDescriptor
    {
        ControllerActionDescriptor GetDescriptor(string controllerName, string actionName);
    }
}