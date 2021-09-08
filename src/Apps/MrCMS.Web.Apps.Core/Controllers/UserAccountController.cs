using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Core.Controllers
{
    public class UserAccountController : MrCMSAppUIController<MrCMSCoreApp>
    {
        private readonly IGetCurrentUser _getCurrentUser;
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly IUserManagementService _userManagementService;

        public UserAccountController(IUserManagementService userManagementService,
            IStringResourceProvider stringResourceProvider,
            IGetCurrentUser getCurrentUser)
        {
            _userManagementService = userManagementService;
            _stringResourceProvider = stringResourceProvider;
            _getCurrentUser = getCurrentUser;
        }


        public async Task<JsonResult> IsUniqueEmail(string email)
        {
            var task = await _getCurrentUser.Get();
            return await _userManagementService.IsUniqueEmail(email, task?.Id)
                ? Json(true)
                : Json(_stringResourceProvider.GetValue("Register Email Already Registered",
                    "Email already registered."));
        }
    }
}