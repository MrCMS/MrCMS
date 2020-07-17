using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.People;
using MrCMS.Web.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Admin.Controllers
{
    public class UserAvatarController : MrCMSAdminController
    {
        private readonly IUserAvatarService _userAvatarService;

        public UserAvatarController(IUserAvatarService userAvatarService)
        {
            _userAvatarService = userAvatarService;
        }
        
        public ActionResult Set(User user)
        {
            return View(user);
        }

        [HttpPost]
        [ActionName("Set")]
        public IActionResult Set_POST(IFormFile file, int id)
        {
            _userAvatarService.SetAvatar(id, file); 
            return Ok(new {id = 1});
        }
    }
}