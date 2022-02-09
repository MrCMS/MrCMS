using Microsoft.AspNetCore.Mvc.Controllers;

namespace MrCMS.Web.Admin.Infrastructure.Routing
{
    public interface IGetAdminActionDescriptor
    {
        ControllerActionDescriptor GetDescriptor(string controllerName, string actionName, string method = GetAdminActionDescriptor.DefaultMethod);
    }
}