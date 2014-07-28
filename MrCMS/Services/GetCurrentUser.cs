using System.Web;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public class GetCurrentUser : IGetCurrentUser
    {
        private readonly IUserService _userService;
        private readonly HttpContextBase _context;

        public GetCurrentUser(IUserService userService, HttpContextBase context)
        {
            _userService = userService;
            _context = context;
        }

        public User Get()
        {
            return _userService.GetCurrentUser(_context);
        }
    }
}