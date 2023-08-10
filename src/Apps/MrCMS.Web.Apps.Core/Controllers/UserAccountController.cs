using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Core.Controllers
{
    public class UserAccountController : MrCMSAppUIController<MrCMSCoreApp>
    {
        private readonly IGetCurrentClaimsPrincipal _getCurrentClaimsPrincipal;
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly IUserManagementService _userManagementService;

        public UserAccountController(IUserManagementService userManagementService,
            IStringResourceProvider stringResourceProvider, IGetCurrentClaimsPrincipal getCurrentClaimsPrincipal)
        {
            _userManagementService = userManagementService;
            _stringResourceProvider = stringResourceProvider;
            _getCurrentClaimsPrincipal = getCurrentClaimsPrincipal;
        }

        public async Task<JsonResult> IsUniqueEmail(string email)
        {
            var user = await _getCurrentClaimsPrincipal.GetPrincipal();
            return await _userManagementService.IsUniqueEmail(email, user.GetUserId())
                ? Json(true)
                : Json(_stringResourceProvider.GetValue("Register Email Already Registered", options =>
                    options.SetDefaultValue("Email already registered.")));
        }
    }
}