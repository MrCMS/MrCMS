using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Core.Controllers
{
    public class RegistrationController : MrCMSAppUIController<MrCMSCoreApp>
    {
        private readonly IUserLookup _userLookup;

        public RegistrationController(IUserLookup userLookup)
        {
            _userLookup = userLookup;
        }


        [Route("check-email-is-not-registered")]
        public async Task<JsonResult> CheckEmailIsNotRegistered([Bind(Prefix = "Input.Email")] string email,
            [Bind(Prefix = "Input.Id")] int? id = null)

        {
            var user = await _userLookup.GetUserByEmail(email);
            return Json(user == null || id.HasValue && user.Id == id);
        }
        
        [Route("check-email-exists")]
        public async Task<JsonResult> CheckEmailIsNotRegisteredMVC(string email,
            int? id = null)

        {
            var user = await _userLookup.GetUserByEmail(email);
            return Json(user == null || id.HasValue && user.Id == id);
        }
    }
}