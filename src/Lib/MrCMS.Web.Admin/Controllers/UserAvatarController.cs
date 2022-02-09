using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class UserAvatarController : MrCMSAdminController
    {
        private readonly IUserAvatarService _userAvatarService;
        private readonly IUserAdminService _userAdminService;

        public UserAvatarController(IUserAvatarService userAvatarService, IUserAdminService userAdminService)
        {
            _userAvatarService = userAvatarService;
            _userAdminService = userAdminService;
        }
        
        public async Task<ActionResult> Set(int id)
        {
            return View(await _userAdminService.GetUser(id));
        }

        [HttpPost]
        [ActionName("Set")]
        public async Task<IActionResult> Set_POST(IFormFile file, int id)
        {
            await _userAvatarService.SetAvatar(id, file); 
            return Ok(new {id = 1});
        }
    }
}