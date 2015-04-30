using System.Web.Mvc;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class RegisteredHandlersController : MrCMSAdminController
    {
        private readonly IRegisteredHandlersAdminService _registeredHandlersAdminService;

        public RegisteredHandlersController(IRegisteredHandlersAdminService registeredHandlersAdminService)
        {
            _registeredHandlersAdminService = registeredHandlersAdminService;
        }

        public ViewResult Index()
        {
            return View(_registeredHandlersAdminService.GetAllHandlers());
        }
    }
}